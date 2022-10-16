using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Rendering;
using Avalonia.Threading;
using RealTimeBitmapAdapter.ImageAdapters;
using RealTimeBitmapAdapter.Models;
using RealTimeBitmapAdapter.ViewModels;
using SkiaSharp;

namespace RealTimeBitmapAdapter.Views;
public partial class MainWindow : Window
{
    private MainWindowViewModel _viewModel = null!;
    private WriteableBitmapAdapter? _wbAdapter;

    private readonly DispatcherTimer _renderTimer;
    private IRenderLoopTask? _renderLoopTask;
    private readonly object _renderLock = new object();

    private DateTime _lastFrameUpdate;
    private int _framesSinceUpdate;

    public MainWindow()
    {
        InitializeComponent();

        _renderTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(1), DispatcherPriority.Render, RenderTimer_Tick);
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        if (DataContext is not MainWindowViewModel viewModel)
            return;

        _viewModel = viewModel;

        RefreshRenderingState();
        StartRendering();

        base.OnDataContextChanged(e);
    }

    private void ApplySettings_Click(object? sender, RoutedEventArgs e)
    {
        lock (_renderLock)
        {
            PauseRendering();

            RefreshRenderingState();

            StartRendering();
        }
    }

    public void RenderFrame()
    {
        lock (_renderLock)
        {
            if (_viewModel is null)
                return;

            // Fill randomized, normalized ([0,1]) colors to the platform-agnostic image
            _viewModel.FillFrame();

            // Map from [0,1] to RGB greyscale and copy into the Avalonia WriteableBitmap
            _wbAdapter?.Invalidate();

            // Update FPS
            _framesSinceUpdate++;
            if (DateTime.Now - _lastFrameUpdate > TimeSpan.FromSeconds(1))
            {
                _viewModel.FramesPerSecond = _framesSinceUpdate;
                _framesSinceUpdate = 0;
                _lastFrameUpdate = DateTime.Now;
            }

            // Render Text Overlay
            if (_viewModel.ShouldShowOverlay)
                RenderOverlay();

            // Let the <Image> control know that the WriteableBitmap has been modified and needs to redraw
            _image.InvalidateVisual();
        }
    }

    private void RenderOverlay()
    {
        if (_wbAdapter?.OutputImage is null)
            return;

        var imageInfo = new SKImageInfo()
        {
            Width = _viewModel.ActualWidth,
            Height = _viewModel.ActualHeight,
            ColorType = SKColorType.Bgra8888,
            AlphaType = SKAlphaType.Premul,
            ColorSpace = SKColorSpace.CreateSrgb()
        };

        using var frameBuffer = _wbAdapter.OutputImage.Lock();
        using var surface = SKSurface.Create(imageInfo, frameBuffer.Address);
        using var paint = new SKPaint()
        {
            TextSize = 36.0f,
            IsAntialias = true,
            Color = new SKColor(0x42, 0x81, 0xA4),
            IsStroke = false
        };

        using var outline = new SKPaint()
        {
            TextSize = 36.0f,
            IsAntialias = false,
            Color = new SKColor(0x00, 0x00, 0x00),
            StrokeWidth = 1.5f,
            IsStroke = true
        };

        var canvas = surface.Canvas;
        canvas.DrawText($"FPS: {_viewModel.FramesPerSecond}", 20, 40, paint);
        canvas.DrawText($"FPS: {_viewModel.FramesPerSecond}", 20, 40, outline);
        canvas.DrawText($"Size: {_viewModel.ActualWidth}x{_viewModel.ActualHeight}", 20, 70, paint);
        canvas.DrawText($"Size: {_viewModel.ActualWidth}x{_viewModel.ActualHeight}", 20, 70, outline);
    }

    private void RenderTimer_Tick(object? sender, EventArgs e) => RenderFrame();

    private void StartRendering()
    {
        if (_viewModel.UseRenderLoop)
            _renderLoopTask = RenderLoopTaskFunc.Add(time => RenderFrame(), null);
        else
            _renderTimer?.Start();
    }

    private void RefreshRenderingState()
    {
        _viewModel.Image = new ColorScaleImage(_viewModel.ImageWidth, _viewModel.ImageHeight);
        _viewModel.ActualWidth = _viewModel.ImageWidth;
        _viewModel.ActualHeight = _viewModel.ImageHeight;

        _wbAdapter?.OutputImage?.Dispose();
        _wbAdapter = new WriteableBitmapAdapter(_viewModel.Image);
        _wbAdapter.UseParallelRenderStrategy = _viewModel.UseParallelStrategy;
        _image.Source = _wbAdapter.OutputImage;
    }

    private void PauseRendering()
    {
        if (_renderLoopTask is not null)
            RenderLoopTaskFunc.Remove(_renderLoopTask);

        _renderTimer?.Stop();
    }
}
