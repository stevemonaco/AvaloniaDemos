using CommunityToolkit.Mvvm.ComponentModel;

namespace Dialogs.ViewModels;

public enum TodoPriority { Low, Medium, High, Urgent }

public partial class TodoViewModel : ObservableObject
{
    [ObservableProperty] private string _activity = "";
    [ObservableProperty] private bool _isCompleted;
    [ObservableProperty] private TodoPriority _priority = TodoPriority.Low;
}
