using CommunityToolkit.Mvvm.ComponentModel;

namespace SkiaBitmapAdapter.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] private SkiaBitmapViewModel _bitmapViewModel = new();
}
