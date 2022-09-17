using System.Globalization;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Media.TextFormatting;

namespace TextOutlineAnimation.Controls;
public sealed partial class OutlinedText : TextBlock
{
    private Geometry? _geometry;

    static OutlinedText()
    {
        ClipToBoundsProperty.OverrideDefaultValue<OutlinedText>(true);

        var items = new AvaloniaProperty[]
        {
            BackgroundProperty, ForegroundProperty, StrokeProperty, StrokeDashArrayProperty, StrokeDashOffsetProperty,
            StrokeJoinProperty, StrokeLineCapProperty, StrokeThicknessProperty
        };

        AffectsRender<OutlinedText>(items);
    }

    protected override void RenderTextLayout(DrawingContext context, Point origin)
    {

        ImmutablePen? pen = null;

        if (Stroke is not null)
        {
            var strokeDashArray = StrokeDashArray;

            ImmutableDashStyle? dashStyle = null;

            if (strokeDashArray is not null && strokeDashArray.Count > 0)
            {
                dashStyle = new ImmutableDashStyle(strokeDashArray, StrokeDashOffset);
            }

            pen = new ImmutablePen(Stroke.ToImmutable(), StrokeThickness, dashStyle, StrokeLineCap, StrokeJoin);
        }

        if (TextWrapping == TextWrapping.NoWrap)
        {
            _geometry = _geometry ?? CreateUnwrappedTextGeometry(Text, origin);
            context.DrawGeometry(Foreground, pen, _geometry!);
        }
        else
        {
            // A better approach to caching and invalidation than used here will be needed for wrapped text
            _geometry = CreateWrappedTextGeometryGroup(TextLayout, origin);
            context.DrawGeometry(Foreground, pen, _geometry);
        }
    }

    private string GetLineString(TextLine line)
    {
        var sb = new StringBuilder();

        foreach (var run in line.TextRuns)
        {
            sb.Append(run.Text.Span);
        }

        return sb.ToString();
    }

    private Geometry? CreateUnwrappedTextGeometry(string? text, Point origin)
    {
        var formattedText = new FormattedText(text ?? "", CultureInfo.GetCultureInfo("en-us"), FlowDirection,
            new Typeface(FontFamily, FontStyle, FontWeight), FontSize, Foreground);

        return formattedText.BuildGeometry(origin);
    }

    private GeometryGroup CreateWrappedTextGeometryGroup(TextLayout layout, Point origin)
    {
        var location = origin;

        var group = new GeometryGroup();

        foreach (var line in TextLayout.TextLines)
        {
            var content = GetLineString(line);
            var geometry = CreateUnwrappedTextGeometry(content, location);

            if (geometry is not null)
            {
                group.Children.Add(geometry);
                location += new Point(0, line.Height);
            }
        }

        return group;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        switch (change.Property.Name)
        {
            case nameof(Stroke):
            case nameof(StrokeJoin):
            case nameof(StrokeDashArray):
            case nameof(StrokeDashOffset):
            case nameof(StrokeThickness):
            case nameof(StrokeLineCap):
                {
                    InvalidateTextLayout();
                    break;
                }
        }
    }
}
