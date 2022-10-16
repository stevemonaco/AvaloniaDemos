using System;
using RealTimeBitmapAdapter.Rng;

namespace RealTimeBitmapAdapter.Models;

/// <summary>
/// Image that contains randomized float values between 0-1 to later be mapped to a real color
/// Some applied uses might include heat maps before being colorized
/// </summary>
public class ColorScaleImage : ImageBase<float>
{
    private XoshiroRandom _xoshiro = new();
    private Random _random = new();

    public ColorScaleImage(int width, int height)
    {
        Image = new float[width * height];
        Width = width;
        Height = height;
    }

    public override void Render()
    {
        if (Image is not null)
        {
            for (int i = 0; i < Image.Length; i++)
            {
                Image[i] = _xoshiro.NextSingle();
            }
        }
    }
}