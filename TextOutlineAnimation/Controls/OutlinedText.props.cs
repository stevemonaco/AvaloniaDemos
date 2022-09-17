using Avalonia.Collections;
using Avalonia.Media;
using Avalonia;
using Avalonia.Controls.Shapes;

namespace TextOutlineAnimation.Controls;
public partial class OutlinedText
{
    /// <summary>
    /// Defines the <see cref="Stroke"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> StrokeProperty =
        AvaloniaProperty.Register<Shape, IBrush?>(nameof(Stroke));

    /// <summary>
    /// Defines the <see cref="StrokeDashArray"/> property.
    /// </summary>
    public static readonly StyledProperty<AvaloniaList<double>?> StrokeDashArrayProperty =
        AvaloniaProperty.Register<Shape, AvaloniaList<double>?>(nameof(StrokeDashArray));

    /// <summary>
    /// Defines the <see cref="StrokeDashOffset"/> property.
    /// </summary>
    public static readonly StyledProperty<double> StrokeDashOffsetProperty =
        AvaloniaProperty.Register<Shape, double>(nameof(StrokeDashOffset));

    /// <summary>
    /// Defines the <see cref="StrokeThickness"/> property.
    /// </summary>
    public static readonly StyledProperty<double> StrokeThicknessProperty =
        AvaloniaProperty.Register<Shape, double>(nameof(StrokeThickness));

    /// <summary>
    /// Defines the <see cref="StrokeLineCap"/> property.
    /// </summary>
    public static readonly StyledProperty<PenLineCap> StrokeLineCapProperty =
        AvaloniaProperty.Register<Shape, PenLineCap>(nameof(StrokeLineCap), PenLineCap.Flat);

    /// <summary>
    /// Defines the <see cref="StrokeJoin"/> property.
    /// </summary>
    public static readonly StyledProperty<PenLineJoin> StrokeJoinProperty =
        AvaloniaProperty.Register<Shape, PenLineJoin>(nameof(StrokeJoin), PenLineJoin.Miter);

    /// <summary>
    /// Gets or sets the <see cref="IBrush"/> that specifies how the shape's outline is painted.
    /// </summary>
    public IBrush? Stroke
    {
        get { return GetValue(StrokeProperty); }
        set { SetValue(StrokeProperty, value); }
    }

    /// <summary>
    /// Gets or sets a collection of <see cref="double"/> values that indicate the pattern of dashes and gaps that is used to outline shapes.
    /// </summary>
    public AvaloniaList<double>? StrokeDashArray
    {
        get { return GetValue(StrokeDashArrayProperty); }
        set { SetValue(StrokeDashArrayProperty, value); }
    }

    /// <summary>
    /// Gets or sets a value that specifies the distance within the dash pattern where a dash begins.
    /// </summary>
    public double StrokeDashOffset
    {
        get { return GetValue(StrokeDashOffsetProperty); }
        set { SetValue(StrokeDashOffsetProperty, value); }
    }

    /// <summary>
    /// Gets or sets the width of the shape outline.
    /// </summary>
    public double StrokeThickness
    {
        get { return GetValue(StrokeThicknessProperty); }
        set { SetValue(StrokeThicknessProperty, value); }
    }

    /// <summary>
    /// Gets or sets a <see cref="PenLineCap"/> enumeration value that describes the shape at the ends of a line.
    /// </summary>
    public PenLineCap StrokeLineCap
    {
        get { return GetValue(StrokeLineCapProperty); }
        set { SetValue(StrokeLineCapProperty, value); }
    }

    /// <summary>
    /// Gets or sets a <see cref="PenLineJoin"/> enumeration value that specifies the type of join that is used at the vertices of a Shape.
    /// </summary>
    public PenLineJoin StrokeJoin
    {
        get { return GetValue(StrokeJoinProperty); }
        set { SetValue(StrokeJoinProperty, value); }
    }
}
