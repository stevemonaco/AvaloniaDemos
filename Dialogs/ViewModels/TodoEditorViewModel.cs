using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dialogs.Abstractions;
using System.Threading.Tasks;

namespace Dialogs.ViewModels;
public partial class TodoEditorViewModel : DialogViewModel<TodoViewModel>
{
    [ObservableProperty] private string _activity = "";
    [ObservableProperty] private bool _isCompleted;
    [ObservableProperty] private TodoPriority _priority = TodoPriority.Low;

    private readonly IInteractionService _interactionService;

    public TodoEditorViewModel(IInteractionService interactionService)
    {
        _interactionService = interactionService;
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

    [RelayCommand]
    public async Task TestAlert()
    {
        var message = $"Activity: {Activity}\nPriority: {Priority}";
        await _interactionService.AlertAsync("Testing Nested Dialogs", message);
    }
}
