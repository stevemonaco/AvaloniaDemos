namespace SkiaRendering.ExpandingCircles;

public record RenderInvalidatedMessage;

public static class Messages
{
    public static RenderInvalidatedMessage RenderInvalidatedMessage { get; } = new();
}