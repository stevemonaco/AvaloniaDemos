namespace SkiaRendering.InfiniteCanvas;

public sealed class UpdateStateEventArgs : EventArgs
{
    public TimeSpan TimeElapsed { get; }
    
    public UpdateStateEventArgs(TimeSpan timeElapsed)
    {
        TimeElapsed = timeElapsed;
    }
}