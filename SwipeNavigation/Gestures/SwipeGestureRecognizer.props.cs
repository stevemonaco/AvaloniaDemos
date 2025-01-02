using Avalonia;

namespace SwipeNavigation.Gestures;
public partial class SwipeGestureRecognizer
{
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

    public static readonly StyledProperty<bool> CanSwipeLeftProperty =
        AvaloniaProperty.Register<SwipeGestureRecognizer, bool>(nameof(CanSwipeLeft));

    /// <summary>
    /// Gets or sets the CanSwipeLeft property. This StyledProperty
    /// indicates ....
    /// </summary>
    public bool CanSwipeLeft
    {
        get => GetValue(CanSwipeLeftProperty);
        set => SetValue(CanSwipeLeftProperty, value);
    }

    public static readonly StyledProperty<bool> CanSwipeRightProperty =
        AvaloniaProperty.Register<SwipeGestureRecognizer, bool>(nameof(CanSwipeRight));

    /// <summary>
    /// Gets or sets the CanSwipeRight  property. This StyledProperty
    /// indicates ....
    /// </summary>
    public bool CanSwipeRight
    {
        get => GetValue(CanSwipeRightProperty);
        set => SetValue(CanSwipeRightProperty, value);
    }

    public static readonly StyledProperty<bool> CanSwipeUpProperty =
        AvaloniaProperty.Register<SwipeGestureRecognizer, bool>(nameof(CanSwipeUp));

    /// <summary>
    /// Gets or sets the CanSwipeUp property. This StyledProperty
    /// indicates ....
    /// </summary>
    public bool CanSwipeUp
    {
        get => GetValue(CanSwipeUpProperty);
        set => SetValue(CanSwipeUpProperty, value);
    }

    public static readonly StyledProperty<bool> CanSwipeDownProperty =
        AvaloniaProperty.Register<SwipeGestureRecognizer, bool>(nameof(CanSwipeDown));

    /// <summary>
    /// Gets or sets the CanSwipeDown property. This StyledProperty
    /// indicates ....
    /// </summary>
    public bool CanSwipeDown
    {
        get => GetValue(CanSwipeDownProperty);
        set => SetValue(CanSwipeDownProperty, value);
    }
}
