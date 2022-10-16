using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using RealTimeBitmapAdapter.Models;

namespace RealTimeBitmapAdapter.ImageAdapters;
public class WriteableBitmapAdapter : BitmapAdapter<ColorScaleImage, WriteableBitmap, float>
{
    public WriteableBitmapAdapter(ColorScaleImage image)
    {
        InputImage = image;
        OutputImage = new WriteableBitmap(new PixelSize(image.Width, image.Height), new Vector(DpiX, DpiY), PixelFormat.Bgra8888, AlphaFormat.Opaque);
    }

    public override void Invalidate()
    {
        Map(0, 0, InputImage.Width, InputImage.Height);
    }

    public override void Invalidate(Rectangle redrawRect)
    {
        var inputRect = new Rectangle(0, 0, InputImage.Width, InputImage.Height);
        var outputRect = new Rectangle(0, 0, OutputImage.PixelSize.Width, OutputImage.PixelSize.Height);

        if (inputRect.Contains(redrawRect) && outputRect.Contains(redrawRect))
        {
            Map(redrawRect.X, redrawRect.Y, redrawRect.Width, redrawRect.Height);
        }
        else
        {
            throw new ArgumentOutOfRangeException($"{nameof(Invalidate)}: Parameter '{nameof(redrawRect)}' {redrawRect} was not contained within '{nameof(InputImage)}' (0, 0, {InputImage.Width}, {InputImage.Height}) and '{nameof(OutputImage)}' (0, 0, {OutputImage.PixelSize.Width}, {OutputImage.PixelSize.Height})");
        }
    }

    public override void Invalidate(int xStart, int yStart, int width, int height)
    {
        var inputRect = new Rectangle(0, 0, InputImage.Width, InputImage.Height);
        var outputRect = new Rectangle(0, 0, OutputImage.PixelSize.Width, OutputImage.PixelSize.Height);
        var redrawRect = new Rectangle(xStart, yStart, width, height);

        if (inputRect.Contains(redrawRect) && outputRect.Contains(redrawRect))
        {
            Map(xStart, yStart, width, height);
        }
        else
        {
            throw new ArgumentOutOfRangeException($"{nameof(Invalidate)}: Calculated parameter '{nameof(redrawRect)}' {redrawRect} was not contained within '{nameof(InputImage)}' (0, 0, {InputImage.Width}, {InputImage.Height}) and '{nameof(OutputImage)}' (0, 0, {OutputImage.PixelSize.Width}, {OutputImage.PixelSize.Height})");
        }
    }

    protected override void Map(int xStart, int yStart, int width, int height)
    {
        if (UseParallelRenderStrategy)
            MapParallel(xStart, yStart, width, height);
        else
            MapSequential(xStart, yStart, width, height);
    }

    private unsafe void MapSequential(int xStart, int yStart, int width, int height)
    {
        using var frameBuffer = OutputImage.Lock();

        var backBuffer = (uint*)frameBuffer.Address.ToPointer();
        var stride = frameBuffer.RowBytes / 4;

        var dest = backBuffer + yStart * stride + xStart;

        for (int y = yStart; y < yStart + height; y++)
        {
            var rowSource = InputImage.GetPixelRowSpan(y);

            for (int x = 0; x < width; x++)
            {
                var color = rowSource[x + xStart];
                dest[x] = TranslateColor(color);
            }

            dest += stride;
        }
    }

    private unsafe void MapParallel(int xStart, int yStart, int width, int height)
    {
        using var frameBuffer = OutputImage.Lock();

        var backBuffer = (uint*)frameBuffer.Address.ToPointer();
        var stride = frameBuffer.RowBytes / 4;

        Parallel.For(yStart, yStart + height, (scanline) =>
        {
            var dest = backBuffer + scanline * stride + xStart;
            var rowSource = InputImage.GetPixelRowSpan(scanline);

            for (int x = 0; x < width; x++)
            {
                var color = rowSource[x + xStart];
                dest[x] = TranslateColor(color);
            }
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint TranslateColor(float input)
    {
        byte channel = (byte)(255 * input);
        uint color = (uint)((0xFF << 24) | (channel << 16) | (channel << 8) | channel);
        return color;
    }
}
