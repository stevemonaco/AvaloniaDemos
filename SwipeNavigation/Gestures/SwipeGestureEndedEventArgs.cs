using Avalonia;
using Avalonia.Interactivity;

namespace SwipeNavigation.Gestures;
public class SwipeGestureEndedEventArgs : RoutedEventArgs
{
    public int Id { get; }
    public Vector? Delta { get; }
    public SwipeDirection Direction { get; }

    public SwipeGestureEndedEventArgs(int id, Vector? delta, SwipeDirection direction) :
        base(CustomGestures.SwipeGestureEndedEvent)
    {
        Id = id;
        Delta = delta;
        Direction = direction;
    }
}