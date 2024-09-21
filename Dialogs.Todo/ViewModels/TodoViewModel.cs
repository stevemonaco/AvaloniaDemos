using CommunityToolkit.Mvvm.ComponentModel;
using Dialogs.Models;

namespace Dialogs.ViewModels;

public partial class TodoViewModel : ObservableObject
{
    [ObservableProperty] private string _activity = "";
    [ObservableProperty] private bool _isCompleted;
    [ObservableProperty] private TodoPriority _priority = TodoPriority.Low;
}
