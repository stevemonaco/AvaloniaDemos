using System;
using System.Collections.Generic;

namespace RealTimeBitmapAdapter.Models;

/// <summary>
/// Provides a generalized base for an image not tied to any framework
/// </summary>
/// <typeparam name="TPixel"></typeparam>
public abstract class ImageBase<TPixel> where TPixel : struct
{
    public TPixel[]? Image { get; init; }

    /// <summary>
    /// Width of image in pixels
    /// </summary>
    public int Width { get; protected set; }
    /// <summary>
    /// Height of image in pixels
    /// </summary>
    public int Height { get; protected set; }

    public abstract void Render();

    public virtual TPixel GetPixel(int x, int y)
    {
        if (Image is null)
            throw new NullReferenceException($"{nameof(GetPixel)} property '{nameof(Image)}' was null");

        if (x >= Width || y >= Height || x < 0 || y < 0)
            throw new ArgumentOutOfRangeException($"{nameof(GetPixel)} ({nameof(x)}: {x}, {nameof(y)}: {y}) were outside the image bounds ({nameof(Width)}: {Width}, {nameof(Height)}: {Height}");

        return Image[x + Width * y];
    }

    public virtual Span<TPixel> GetPixelSpan()
    {
        if (Image is null)
            throw new NullReferenceException($"{nameof(GetPixel)} property '{nameof(Image)}' was null");

        return new Span<TPixel>(Image);
    }

    public virtual Span<TPixel> GetPixelRowSpan(int y)
    {
        if (Image is null)
            throw new NullReferenceException($"{nameof(GetPixel)} property '{nameof(Image)}' was null");

        if (y >= Height || y < 0)
            throw new ArgumentOutOfRangeException($"{nameof(GetPixel)} {nameof(y)}: {y} was outside the image bounds ({nameof(Height)}: {Height}");

        return new Span<TPixel>(Image, Width * y, Width);
    }

    public virtual IEnumerable<TPixel> Pixels()
    {
        if (Image is null)
            throw new NullReferenceException($"{nameof(GetPixel)} property '{nameof(Image)}' was null");

        return Image;
    }

    /// <summary>
    /// Sets a pixel's color at the specified pixel coordinate
    /// </summary>
    /// <param name="x">x-coordinate in pixel coordinates</param>
    /// <param name="y">y-coordinate in pixel coordinates</param>
    /// <param name="color">New color of the pixel</param>
    public virtual void SetPixel(int x, int y, TPixel color)
    {
        if (Image is null)
            throw new NullReferenceException($"{nameof(GetPixel)} property '{nameof(Image)}' was null");

        if (x >= Width || y >= Height || x < 0 || y < 0)
            throw new ArgumentOutOfRangeException($"{nameof(GetPixel)} ({nameof(x)}: {x}, {nameof(y)}: {y}) were outside the image bounds ({nameof(Width)}: {Width}, {nameof(Height)}: {Height}");

        Image[x + Width * y] = color;
    }
}