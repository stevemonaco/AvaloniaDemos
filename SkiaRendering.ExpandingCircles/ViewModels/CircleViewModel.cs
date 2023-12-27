using CommunityToolkit.Mvvm.ComponentModel;
using Color = Avalonia.Media.Color; // You shouldn't use Avalonia types in VM, but kept simpler for the demo

namespace SkiaRendering.ExpandingCircles.ViewModels;
public sealed partial class CircleViewModel : ObservableElement
{
    [ObservableProperty] private float _x;
    [ObservableProperty] private float _y;
    [ObservableProperty] private float _radius;

    [ObservableProperty] private Color _fillColor;
    [ObservableProperty] private Color _strokeColor;

    public CircleViewModel(float x, float y, float radius, Color fillColor, Color strokeColor)
    {
        _x = x;
        _y = y;
        _radius = radius;

        _fillColor = fillColor;
        _strokeColor = strokeColor;
    }
}
