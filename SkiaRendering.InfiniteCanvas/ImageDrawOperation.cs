using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;

namespace SkiaRendering.InfiniteCanvas;

internal sealed class ImageDrawOperation : ICustomDrawOperation
{
    private readonly WriteableBitmap _bitmap;
    private readonly int _imageWidth;
    private readonly int _imageHeight;

    public ImageDrawOperation(WriteableBitmap bitmap, int imageWidth, int imageHeight)
    {
        _bitmap = bitmap;
        _imageWidth = imageWidth;
        _imageHeight = imageHeight;
        Bounds = new Rect(0, 0, imageWidth, imageHeight);
    }

    public Rect Bounds { get; }

    public void Dispose() { }

    public bool Equals(ICustomDrawOperation? other) => false;
    public bool HitTest(Point p) => false;

    public unsafe void Render(ImmediateDrawingContext context)
    {
        // var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
        // if (leaseFeature is null)
        //     return;
        //
        // using var lease = leaseFeature.Lease();
        // using var bitmap = new SKBitmap();
        //
        // var canvas = lease.SkCanvas;
        // canvas.Save();
        //
        // fixed (void* p = _pixels)
        // {
        //     var bufferPointer = new nint(p);
        //     var info = new SKImageInfo(_imageWidth, _imageHeight, SKColorType.Bgra8888, SKAlphaType.Premul);
        //     var result = bitmap.InstallPixels(info, bufferPointer, info.RowBytes);
        // }
        //
        // context.DrawBitmap(_bitmap, new SKPoint(0, 0));
        //
        // canvas.DrawBitmap(bitmap, new SKPoint(0, 0));
        // canvas.Restore();
    }
}