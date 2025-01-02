using Avalonia.Input.GestureRecognizers;
using Avalonia.Input;
using Avalonia;

namespace SwipeNavigation.Gestures;

public enum SwipeDirection
{
    Left,
    Right,
    Bottom,
    Top,
    None
}

/// <summary>
/// Implements a swipe gesture recognizer that can detect swipe gestures on top of a page
/// </summary>
public partial class SwipeGestureRecognizer : GestureRecognizer
{
    private Point _initialPosition;
    private int _gestureId;
    private IPointer? _tracking;
    private bool _pullInProgress;
    private SwipeDirection _expectedDirection = SwipeDirection.None;

    public SwipeGestureRecognizer() { }

    protected override void PointerCaptureLost(IPointer pointer)
    {
        if (_tracking == pointer)
        {
            EndSwipe(null);
        }
    }

    protected override void PointerMoved(PointerEventArgs e)
    {
        if (_tracking == e.Pointer && Target is Visual visual)
        {
            var currentPosition = e.GetPosition(visual);
            Capture(e.Pointer);

            var delta = new Vector(currentPosition.X - _initialPosition.X, currentPosition.Y - _initialPosition.Y);
            _expectedDirection = CalculateDirection(delta);

            _pullInProgress = true;
            var pullEventArgs = new SwipeGestureEventArgs(_gestureId, delta, _expectedDirection);
            Target?.RaiseEvent(pullEventArgs);

            e.Handled = pullEventArgs.Handled;
        }
    }

    protected override void PointerPressed(PointerPressedEventArgs e)
    {
        if (Target != null && Target is Visual visual && (e.Pointer.Type == PointerType.Touch || e.Pointer.Type == PointerType.Pen))
        {
            var position = e.GetPosition(visual);

            _gestureId = SwipeGestureEventArgs.GetNextFreeId();
            _tracking = e.Pointer;
            _initialPosition = position;
        }
    }

    protected override void PointerReleased(PointerReleasedEventArgs e)
    {
        if (_tracking == e.Pointer && _pullInProgress && Target is Visual visual)
        {
            var currentPosition = e.GetPosition(visual);
            var delta = new Vector(currentPosition.X - _initialPosition.X, currentPosition.Y - _initialPosition.Y);
            _expectedDirection = CalculateDirection(delta);

            EndSwipe(delta);
        }
    }

    private SwipeDirection CalculateDirection(Vector delta)
    {
        if (delta.X <= -SwipeThreshold && CanSwipeLeft)
            return SwipeDirection.Left;
        else if (delta.X >= SwipeThreshold && CanSwipeRight)
            return SwipeDirection.Right;
        else if (delta.Y <= -SwipeThreshold && CanSwipeUp)
            return SwipeDirection.Top;
        else if (delta.Y >= SwipeThreshold && CanSwipeDown)
            return SwipeDirection.Bottom;
        else
            return SwipeDirection.None;
    }

    private void EndSwipe(Vector? delta)
    {
        _tracking = null;
        _initialPosition = default;
        _pullInProgress = false;

        Target?.RaiseEvent(new SwipeGestureEndedEventArgs(_gestureId, delta, _expectedDirection));
    }
}
