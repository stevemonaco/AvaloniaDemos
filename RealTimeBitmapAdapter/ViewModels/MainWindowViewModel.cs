using RealTimeBitmapAdapter.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RealTimeBitmapAdapter.ViewModels;
public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private int _framesPerSecond;
    [ObservableProperty] private int _imageWidth = 600;
    [ObservableProperty] private int _imageHeight = 600;
    [ObservableProperty] private bool _useParallelStrategy = false;
    [ObservableProperty] private bool _shouldShowOverlay = false;

    [ObservableProperty] private int _actualWidth = 600;
    [ObservableProperty] private int _actualHeight = 600;

    public ColorScaleImage Image { get; set; }

    public MainWindowViewModel()
    {
        Image = new ColorScaleImage(ImageWidth, ImageHeight);
    }

    public void FillFrame()
    {
        Image.Render();
    }
}
