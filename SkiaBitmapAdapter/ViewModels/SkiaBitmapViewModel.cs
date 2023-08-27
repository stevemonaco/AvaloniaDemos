using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace SkiaBitmapAdapter.ViewModels;
public readonly record struct ColorBgra(byte B, byte G, byte R, byte A);

public partial class SkiaBitmapViewModel : ObservableObject
{
    [ObservableProperty] private int _framesPerSecond;

    public int ImageWidth { get; } = 1920;
    public int ImageHeight { get; } = 1080;

    public ColorBgra[] Image => _image;
    private ColorBgra[] _image;

    public Action? OnImageChanged { get; set; }

    private ColorBgra[] _palette;
    private int _bandWidth = 32;
    private int _scrollY = 0;

    public SkiaBitmapViewModel()
    {
        _image = new ColorBgra[ImageWidth * ImageHeight];
        _palette = new ColorBgra[]
        {
            MakeRgba(0xFF, 0x61, 0xAB, 255), MakeRgba(0xFF, 0x61, 0x76, 255),
            MakeRgba(0xFF, 0x81, 0x61, 255), MakeRgba(0xFF, 0xB5, 0x61, 255),
            MakeRgba(0xFF, 0xEA, 0x62, 255), MakeRgba(0xDF, 0xFF, 0x61, 255),
            MakeRgba(0xAB, 0xFF, 0x61, 255), MakeRgba(0x76, 0xFF, 0x61, 255),
            MakeRgba(0x61, 0xFF, 0x81, 255), MakeRgba(0x61, 0xFF, 0xB5, 255),
        };

        NextFrame();
    }

    public void NextFrame()
    {
        _scrollY++;
        _scrollY %= _bandWidth * _palette.Length;

        for (int y = 0; y < ImageHeight; y++)
        {
            var index = ((_scrollY + y) / _bandWidth) % _palette.Length;

            var color = _palette[index];
            for (int x = 0; x < ImageWidth; x++)
            {
                _image[y * ImageWidth + x] = color;
            }
        }

        OnImageChanged?.Invoke();
    }

    private ColorBgra MakeRgba(byte r, byte g, byte b, byte a) => new ColorBgra(b, g, r, a);

    private ColorBgra MakeRandomColor()
    {
        return new ColorBgra((byte)Random.Shared.Next(256), (byte)Random.Shared.Next(256), (byte)Random.Shared.Next(256), 255);
    }
}
