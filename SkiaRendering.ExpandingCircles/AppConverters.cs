using Avalonia.Data.Converters;

namespace SkiaRendering.ExpandingCircles;

public static class AppConverters
{
    public static IValueConverter DoubleToPercent { get; } =
        new FuncValueConverter<double, string>(x => $"{(x * 100):0.}%");
}