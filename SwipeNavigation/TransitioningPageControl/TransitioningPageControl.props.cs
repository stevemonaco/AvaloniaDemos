using Avalonia;
using Avalonia.Controls;
using Avalonia.Metadata;
using SwipeNavigation.Gestures;

namespace SwipeNavigation.Controls;
public partial class TransitioningPageControl
{
    /// <summary>
    /// Gets the children which are used to transition across during page swipes.
    /// These controls are fully realized and cached, although only a maximum of two will be 
    /// in the visual and logical trees at once
    /// </summary>
    [Content]
    public Avalonia.Controls.Controls Children { get; } = [];

    public static readonly StyledProperty<Control?> ActivePageProperty =
        AvaloniaProperty.Register<TransitioningPageControl, Control?>(nameof(ActivePage));

    /// <summary>
    /// Page this is active and/or being transitioned into.
    /// indicates ....
    /// </summary>
    public Control? ActivePage
    {
        get => GetValue(ActivePageProperty);
        set => SetValue(ActivePageProperty, value);
    }

    public static readonly StyledProperty<Control?> TransitioningPageProperty =
        AvaloniaProperty.Register<TransitioningPageControl, Control?>(nameof(TransitioningPage));

    /// <summary>
    /// Page that is transitioning out of screen.
    /// </summary>
    public Control? TransitioningPage 
    {
        get => GetValue(TransitioningPageProperty);
        set => SetValue(TransitioningPageProperty, value);
    }

    public static readonly StyledProperty<int> SwipeThresholdProperty =
    AvaloniaProperty.Register<SwipeGestureRecognizer, int>(nameof(SwipeThreshold), 50);

    /// <summary>
    /// Gets or sets the threshold in pixels that must be exceeded for a swipe to have its Direction set.
    /// </summary>
    public int SwipeThreshold
    {
        get => GetValue(SwipeThresholdProperty);
        set => SetValue(SwipeThresholdProperty, value);
    }
}
