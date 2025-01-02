using Avalonia.Interactivity;

namespace SwipeNavigation.Gestures;
public static class CustomGestures
{
    public static readonly RoutedEvent<SwipeGestureEventArgs> SwipeGestureEvent =
        RoutedEvent.Register<SwipeGestureEventArgs>(
            "Swipe", RoutingStrategies.Bubble, typeof(CustomGestures));

    public static readonly RoutedEvent<SwipeGestureEndedEventArgs> SwipeGestureEndedEvent =
        RoutedEvent.Register<SwipeGestureEndedEventArgs>(
            "SwipeEnded", RoutingStrategies.Bubble, typeof(CustomGestures));
}
