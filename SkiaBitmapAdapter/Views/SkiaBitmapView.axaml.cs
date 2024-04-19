using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;
using SkiaBitmapAdapter.ViewModels;
using SkiaSharp;
using System;

namespace SkiaBitmapAdapter.Views;

public partial class SkiaBitmapView : UserControl
{
    private SkiaBitmapViewModel? _viewModel;
    private DateTime _lastFrameUpdate;
    private int _framesSinceUpdate;
    private TopLevel? _topLevel;

    public SkiaBitmapView()
    {
        InitializeComponent();
        DataContextChanged += SkiaImageView_DataContextChanged;
        Loaded += SkiaImageView_Loaded;
    }

    private void SkiaImageView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _topLevel = TopLevel.GetTopLevel(this)!;
        _topLevel.RequestAnimationFrame(PrepareFrame);
    }

    private void PrepareFrame(TimeSpan dt)
    {
        if (_viewModel is null)
            return;

        _framesSinceUpdate++;
        if (DateTime.Now - _lastFrameUpdate > TimeSpan.FromSeconds(1))
        {
            _viewModel.FramesPerSecond = _framesSinceUpdate;
            _framesSinceUpdate = 0;
            _lastFrameUpdate = DateTime.Now;
        }

        _viewModel.NextFrame();
        _topLevel?.RequestAnimationFrame(PrepareFrame);
    }

    private void SkiaImageView_DataContextChanged(object? sender, System.EventArgs e)
    {
        if (DataContext is not SkiaBitmapViewModel viewModel)
            return;

        _viewModel = viewModel;
    }

    public override void Render(DrawingContext context)
    {
        if (_viewModel is null)
            return;

        context.Custom(new ImageDrawOperation(_viewModel.Image, _viewModel.ImageWidth, _viewModel.ImageHeight));
        Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Background);
    }
}

file class ImageDrawOperation : ICustomDrawOperation
{
    private readonly ColorBgra[] _pixels;
    private readonly int _imageWidth;
    private readonly int _imageHeight;

    public ImageDrawOperation(ColorBgra[] pixels, int imageWidth, int imageHeight)
    {
        _pixels = pixels;
        _imageWidth = imageWidth;
        _imageHeight = imageHeight;
        Bounds = new Rect(0, 0, imageWidth, imageHeight);
    }

    public Rect Bounds { get; }

    public void Dispose() { }

    public bool Equals(ICustomDrawOperation? other) => false;
    public bool HitTest(Point p) => false;

    public unsafe void Render(ImmediateDrawingContext context)
    {
        var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
        if (leaseFeature is null)
            return;

        using var lease = leaseFeature.Lease();
        using var bitmap = new SKBitmap();

        var canvas = lease.SkCanvas;
        canvas.Save();

        fixed (void* p = _pixels)
        {
            var bufferPointer = new nint(p);
            var info = new SKImageInfo(_imageWidth, _imageHeight, SKColorType.Bgra8888, SKAlphaType.Premul);
            var result = bitmap.InstallPixels(info, bufferPointer, info.RowBytes);
        }

        canvas.DrawBitmap(bitmap, new SKPoint(0, 0));
        canvas.Restore();
    }
}