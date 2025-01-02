using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using System.Collections.Specialized;
using System;
using System.ComponentModel;
using Avalonia.Input;
using Avalonia.Rendering;
using Avalonia.Rendering.Composition;
using Avalonia.Animation.Easings;
using System.Numerics;
using SwipeNavigation.Gestures;
using System.Linq;
using Avalonia.Controls.Metadata;

namespace SwipeNavigation.Controls;

[PseudoClasses(_previewSwipeLeft, _previewSwipeRight)]
public partial class TransitioningPageControl : Control, ICustomHitTest
{
    private int? _activeIndex;
    private SwipeGestureRecognizer _swipeRecognizer;

    private static IEasing _transitionEasing = new SplineEasing(0.25, 0.1, 0.25, 1.0);
    private static IEasing _snapBackEasing = new SplineEasing(0.25, 0.1, 0.25, 1.0);
    private bool CanSwipe => Children.Count > 1;

    private const string _previewSwipeLeft = ":preview-swipe-left";
    private const string _previewSwipeRight = ":preview-swipe-right";

    static TransitioningPageControl()
    {
        FocusableProperty.OverrideDefaultValue<TransitioningPageControl>(true);
        FocusAdornerProperty.OverrideDefaultValue<TransitioningPageControl>(null);
    }

    public TransitioningPageControl()
    {
        Children.CollectionChanged += ChildrenChanged;
        Children.PropertyChanged += ChildrenPropertyChanged;

        AddHandler(CustomGestures.SwipeGestureEvent, OnPageSwipe);
        AddHandler(CustomGestures.SwipeGestureEndedEvent, OnPageSwipeEnded);

        // Could be attached from XAML, too, but makes more sense here
        _swipeRecognizer = new SwipeGestureRecognizer()
        {
            CanSwipeLeft = true,
            CanSwipeRight = true
        };

        GestureRecognizers.Add(_swipeRecognizer);
    }

    private void OnPageSwipe(object? sender, SwipeGestureEventArgs e)
    {
        if (ActivePage is null || !CanSwipe)
            return;

        if (e.Delta.HasValue)
        {
            var pageVisual = ElementComposition.GetElementVisual(ActivePage);

            if (pageVisual is not null)
                pageVisual.Offset = new Vector3((float)e.Delta.Value.X, 0, 0);
        }

        SetSwipePseudoClasses(e.ExpectedDirection);
    }

    private void OnPageSwipeEnded(object? sender, SwipeGestureEndedEventArgs e)
    {
        SetSwipePseudoClasses(SwipeDirection.None);

        if (ActivePage is null || !CanSwipe)
            return;

        if (e.Direction == SwipeDirection.Left)
        {
            NavigateToNext(e.Delta!.Value.X);
        }
        else if (e.Direction == SwipeDirection.Right)
        {
            NavigateToPrevious(e.Delta!.Value.X);
        }
        else if (e.Direction == SwipeDirection.None && e.Delta.HasValue)
        {
            var pageVisual = ElementComposition.GetElementVisual(ActivePage);

            var _resetAnimation = pageVisual!.Compositor!.CreateVector3KeyFrameAnimation();
            _resetAnimation.Target = "Offset";

            if (e.Delta.HasValue)
            _resetAnimation.InsertKeyFrame(0f, new Vector3((float)e.Delta.Value.X, 0, 0), _snapBackEasing);
            _resetAnimation.InsertKeyFrame(1f, new Vector3(0, 0, 0), _snapBackEasing);
            _resetAnimation.Duration = TimeSpan.FromSeconds(2);

            pageVisual.StartAnimation("Offset", _resetAnimation);
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Left)
        {
            NavigateToPrevious();
        }
        else if (e.Key == Key.Right)
        {
            NavigateToNext();
        }
        else
        {
            base.OnKeyDown(e);
        }
    }

    private void NavigateToPrevious(double? swipeDisplacement = null)
    {
        if (_activeIndex is null || !CanSwipe)
            return;

        _activeIndex = _activeIndex.Value == 0 ? 2 : _activeIndex.Value - 1;
        TransitioningPage = ActivePage;
        ActivePage = Children[_activeIndex.Value];

        AnimateTransitionToPage(ActivePage, TransitioningPage!, true, swipeDisplacement);
    }

    private void NavigateToNext(double? swipeDisplacement = null)
    {
        if (_activeIndex is null || !CanSwipe)
            return;

        _activeIndex = (_activeIndex.Value + 1) % 3;
        TransitioningPage = ActivePage;
        ActivePage = Children[_activeIndex.Value];

        AnimateTransitionToPage(ActivePage, TransitioningPage!, false, swipeDisplacement);
    }

    private void AnimateTransitionToPage(Control to, Control from, bool shouldSlideRight, double? swipeDisplacement = null)
    {
        var toVisual = ElementComposition.GetElementVisual(to);
        var fromVisual = ElementComposition.GetElementVisual(from);

        if (toVisual is null || fromVisual is null)
            return;

        var slideTo = shouldSlideRight ? Slide(toVisual, (float)-Bounds.Width, 0) : Slide(toVisual, (float)Bounds.Width, 0);
        var slideFrom = shouldSlideRight ? Slide(fromVisual, (float?)swipeDisplacement, (float)Bounds.Width) : Slide(fromVisual, (float?)swipeDisplacement, (float)-Bounds.Width);

        toVisual.StartAnimation("Offset", slideTo);
        fromVisual.StartAnimation("Offset", slideFrom);

        Vector3KeyFrameAnimation Slide(CompositionVisual visual, float? startX, float endX)
        {
            var slideAnimation = visual.Compositor.CreateVector3KeyFrameAnimation();
            slideAnimation.Target = "Offset";

            if (startX.HasValue)
                slideAnimation.InsertKeyFrame(0f, new Vector3(startX.Value, 0, 0), _transitionEasing);
            else
                slideAnimation.InsertKeyFrame(0f, ToVector3(visual.Offset), _transitionEasing);

            slideAnimation.InsertKeyFrame(1f, new Vector3(endX, 0, 0), _transitionEasing);
            slideAnimation.Duration = TimeSpan.FromSeconds(1);
            return slideAnimation;
        }
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        InvalidateMeasure();
    }

    private static void OnActivePageChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Sender is not TransitioningPageControl tpc || e.NewValue is not Control newPage)
            return;

        tpc.VisualChildren.Clear();
        tpc.LogicalChildren.Clear();

        tpc.LogicalChildren.Add(newPage);
        tpc.VisualChildren.Add(newPage);

        if (tpc.TransitioningPage is not null)
        {
            tpc.LogicalChildren.Add(tpc.TransitioningPage);
            tpc.VisualChildren.Add(tpc.TransitioningPage);
        }

        tpc.InvalidateMeasure();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (ActivePage is not null)
        {
            ActivePage.Measure(availableSize);
        }

        return availableSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (ActivePage is not null)
        {
            ActivePage.Arrange(new Rect(finalSize));
        }

        return finalSize;
    }

    private void ChildrenPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        _activeIndex = null;

        if (Children.Count > 0)
        {
            _activeIndex = 0;
            ActivePage = Children[0];

            InvalidateMeasure();
        }
    }

    protected virtual void ChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_activeIndex is null || ActivePage is null)
            return;

        var pageIndex = Children.IndexOf(ActivePage);

        if (Children.Count == 0)
        {
            _activeIndex = null;
        }
        else if (Children.Count == 1)
        {
            ActivePage = Children.First();
            _activeIndex = 0;
        }
        else if (pageIndex == -1) // ActivePage removed, fallback to previous page
        {
            pageIndex = Math.Max(0, _activeIndex.Value - 1);
            ActivePage = Children[pageIndex];
        }
        else
        {
            _activeIndex = pageIndex;
        }

        InvalidateMeasure();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == ActivePageProperty)
        {
            OnActivePageChanged(change);
        }
        else if (change.Property == SwipeThresholdProperty)
        {
            _swipeRecognizer.SwipeThreshold = (int)change.NewValue!;
        }
        else
        {
            base.OnPropertyChanged(change);
        }
    }

    private void SetSwipePseudoClasses(SwipeDirection direction)
    {
        PseudoClasses.Set(_previewSwipeLeft, direction == SwipeDirection.Left);
        PseudoClasses.Set(_previewSwipeRight, direction == SwipeDirection.Right);
    }

    private Vector3 ToVector3(Vector3D v) => new Vector3((float)v.X, (float)v.Y, (float)v.Z);

    public bool HitTest(Point point) => true;
}
