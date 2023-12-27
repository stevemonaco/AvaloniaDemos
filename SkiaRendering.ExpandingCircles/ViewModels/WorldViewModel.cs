using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using AvaColor = Avalonia.Media.Color; // You shouldn't use Avalonia types in VM, but kept simpler for the demo

namespace SkiaRendering.ExpandingCircles.ViewModels;
public sealed partial class WorldViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<CircleViewModel> _circles = new();
    [ObservableProperty] private float _left = 0f;
    [ObservableProperty] private float _width = 1000f;
    [ObservableProperty] private float _top = 0f;
    [ObservableProperty] private float _height = 1000f;

    public WorldViewModel()
    {
        for (int i = 0; i < 50; i++)
            AddRandomCircle();
    }

    public void AddRandomCircle()
    {
        var x = Random.Shared.NextDouble() * Width + Left;
        var y = Random.Shared.NextDouble() * Height + Top;
        AddRandomCircle((float)x, (float)y);
    }

    public void AddRandomCircle(float x, float y)
    {
        var radius = Random.Shared.NextDouble() * 20;
        var fillColor = CreateRandomColor();
        var strokeColor = CreateRandomColor();

        var circle = new CircleViewModel(x, y, (float)radius, fillColor, strokeColor);
        Circles.Add(circle);
    }

    public AvaColor CreateRandomColor() =>
        new AvaColor(255, (byte)Random.Shared.Next(0, 255), (byte)Random.Shared.Next(0, 255), (byte)Random.Shared.Next(0, 255));
}
