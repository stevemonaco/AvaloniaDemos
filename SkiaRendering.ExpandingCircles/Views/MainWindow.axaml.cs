using Avalonia.Controls;
using Avalonia.Input;
using SkiaRendering.ExpandingCircles.ViewModels;
using SkiaSharp;
using System.Diagnostics.CodeAnalysis;
using System;
using System.Linq;
using Avalonia;
using Avalonia.Interactivity;
using SkiaRendering.InfiniteCanvas;
using AvaColor = Avalonia.Media.Color;
using AvaPoint = Avalonia.Point;

namespace SkiaRendering.ExpandingCircles.Views;
public partial class MainWindow : Window
{
    public MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext!;
    public WorldViewModel World => ViewModel.World;

    public MainWindow()
    {
        InitializeComponent();
        infiniteCanvas.PaintSurface += Canvas_PaintSurface;
        infiniteCanvas.UpdateState += Canvas_UpdateState;

        infiniteCanvas.PointerPressed += Canvas_PointerPressed;
        infiniteCanvas.PointerReleased += Canvas_PointerReleased;
        infiniteCanvas.PointerMoved += Canvas_PointerMoved;
        infiniteCanvas.PointerWheelChanged += Canvas_PointerWheelChanged;

        infiniteCanvas.PropertyChanged += Canvas_PropertyChanged;
    }

    private void Canvas_UpdateState(object? sender, UpdateStateEventArgs e)
    {
        foreach (var circle in World.Circles)
        {
            circle.Radius += 0.1f;
        }
    }

    private void Canvas_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property.Name is "OffsetX" or "OffsetY")
        {
            canvasOriginText.Text = $"Origin: ({infiniteCanvas.OffsetX}, {infiniteCanvas.OffsetY})";
        }
    }

    private void Canvas_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Pointer.Type != PointerType.Mouse)
            return;

        var pointer = e.GetCurrentPoint(infiniteCanvas);
        var localPoint = infiniteCanvas.ScreenToLocalPoint(pointer.Position);

        if (pointer.Properties.IsLeftButtonPressed) // Remove circle if one is under cursor, otherwise create a random circle at cursor
        {
            if (TryPickCircle(localPoint, out var circleViewModel))
            {
                World.Circles.Remove(circleViewModel);
                infiniteCanvas.Invalidate();
                e.Handled = true;
            }
            else
            {
                World.AddRandomCircle((float)localPoint.X, (float)localPoint.Y);
                infiniteCanvas.Invalidate();
                e.Handled = true;
            }
        }
        else if (pointer.Properties.IsMiddleButtonPressed) // Start panning
        {
            infiniteCanvas.StartPan(pointer.Position);
            e.Handled = true;
        }
        else if (pointer.Properties.IsRightButtonPressed) // Enlarge circle under cursor
        {
            if (TryPickCircle(localPoint, out var circleViewModel))
            {
                circleViewModel.Radius *= 1.5f;
                infiniteCanvas.Invalidate();
                e.Handled = true;
            }
        }
    }

    private void Canvas_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.Pointer.Type == PointerType.Mouse) // Stop panning
        {
            if (e.InitialPressMouseButton == MouseButton.Middle)
            {
                infiniteCanvas.EndPan();
                e.Handled = true;
            }
        }
    }

    private void Canvas_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (e.Pointer.Type != PointerType.Mouse)
            return;

        var pointer = e.GetCurrentPoint(infiniteCanvas);
        var localPoint = infiniteCanvas.ScreenToLocalPoint(pointer.Position);

        mousePositionText.Text = $"Mouse: ({localPoint.X}, {localPoint.Y})";
    }

    private void Canvas_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);

        var location = e.GetPosition(infiniteCanvas);

        if (e.Delta.Y > 0)
        {
            infiniteCanvas.ZoomInOnCenter(location);
        }
        else if (e.Delta.Y < 0)
        {
            infiniteCanvas.ZoomOutOnCenter(location);
        }

        e.Handled = true;
    }

    private void Canvas_PaintSurface(object? sender, InfiniteCanvas.SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;

        foreach (var circle in World.Circles) // Rendering is back-to-front
        {
            // You will want to cache the SKPaint in a real app instead of recreating each frame
            using var fill = CreateFill(ToSKColor(circle.FillColor));
            canvas.DrawCircle(new SKPoint(circle.X, circle.Y), circle.Radius, fill);

            using var stroke = CreateStroke(ToSKColor(circle.StrokeColor), 2);
            canvas.DrawCircle(new SKPoint(circle.X, circle.Y), circle.Radius, stroke);
        }
    }

    private bool TryPickCircle(AvaPoint point, [MaybeNullWhen(false)] out CircleViewModel viewModel)
    {
        viewModel = null;

        foreach (var circle in World.Circles.Reverse()) // Picking is front-to-back
        {
            var squareDiff = (point.X - circle.X) * (point.X - circle.X) + (point.Y - circle.Y) * (point.Y - circle.Y);
            var dist = Math.Sqrt(squareDiff);

            if (dist <= circle.Radius)
            {
                viewModel = circle;
                return true;
            }
        }

        return false;
    }

    private SKPaint CreateStroke(SKColor color, float strokeWidth) => new SKPaint()
    {
        Color = color,
        IsStroke = true,
        IsAntialias = true,
        StrokeWidth = strokeWidth
    };

    private SKPaint CreateFill(SKColor color) => new SKPaint()
    {
        Color = color,
        IsAntialias = true
    };

    private SKColor ToSKColor(AvaColor color) => new SKColor(color.R, color.G, color.B, color.A);

    private void PauseBox_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (infiniteCanvas is null)
            return;

        if (sender is CheckBox { IsChecked: true })
        {
            infiniteCanvas.RenderTrigger = RenderTrigger.Invalidation;
        }
        else if (sender is CheckBox { IsChecked: false })
        {
            infiniteCanvas.RenderTrigger = RenderTrigger.Continuous;
        }
    }
}