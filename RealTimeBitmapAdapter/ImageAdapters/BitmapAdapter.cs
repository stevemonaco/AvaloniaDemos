using RealTimeBitmapAdapter.Models;
using System.Drawing;

namespace RealTimeBitmapAdapter.ImageAdapters;
public abstract class BitmapAdapter<TInput, TOutput, TPixel>
    where TInput : ImageBase<TPixel>
    where TPixel : struct
{
    public TInput InputImage { get; protected set; } = default!;
    public TOutput OutputImage { get; protected set; } = default!;

    public bool UseParallelRenderStrategy { get; set; }

    public int DpiX { get; protected set; } = 96;
    public int DpiY { get; protected set; } = 96;

    public abstract void Invalidate();
    public abstract void Invalidate(Rectangle redrawRect);
    public abstract void Invalidate(int xStart, int yStart, int width, int height);

    /// <summary>
    /// Maps the InputImage into the OutputImage for the specified region
    /// </summary>
    protected abstract void Map(int xStart, int yStart, int width, int height);
}
