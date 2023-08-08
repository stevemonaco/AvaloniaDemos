using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Dialogs.ViewModels;
public partial class TodoEditorViewModel : DialogViewModel<TodoViewModel>
{
    [ObservableProperty] private string _activity = "";
    [ObservableProperty] private bool _isCompleted;
    //[ObservableProperty] private TodoPriority _priority = TodoPriority.Low;
    private TodoPriority _priority = TodoPriority.Low;
    public TodoPriority Priority
    {
        get => _priority;
        set => SetProperty(ref _priority, value);
    }


    protected override void Accept()
    {
        RequestResult = new TodoViewModel()
        {
            Activity = Activity,
            IsCompleted = IsCompleted,
            Priority = Priority
        };
    }
}
