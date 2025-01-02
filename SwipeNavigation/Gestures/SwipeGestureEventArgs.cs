using Avalonia;
using Avalonia.Interactivity;

namespace SwipeNavigation.Gestures;
public class SwipeGestureEventArgs : RoutedEventArgs
{
    public int Id { get; }
    public Vector? Delta { get; }
    public SwipeDirection ExpectedDirection { get; }

    private static int _nextId = 1;

    internal static int GetNextFreeId() => _nextId++;

    public SwipeGestureEventArgs(int id, Vector? delta, SwipeDirection expectedDirection)
        : base(CustomGestures.SwipeGestureEvent)
    {
        Id = id;
        Delta = delta;
        ExpectedDirection = expectedDirection;
    }
}
