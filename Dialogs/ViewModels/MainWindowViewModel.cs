using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dialogs.Abstractions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Dialogs.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<TodoViewModel> _todos = new()
    {
        new TodoViewModel
        {
            Activity = "Mow the Lawn",
            Priority = TodoPriority.Medium
        },
        new TodoViewModel
        {
            Activity = "Buy Groceries",
            Priority = TodoPriority.High,
        },
        new TodoViewModel
        {
            Activity = "Fix the Door Hinge",
            Priority = TodoPriority.Low,
        },
        new TodoViewModel
        {
            Activity = "Finish this Demo",
            Priority = TodoPriority.Urgent,
        }
    };

    public IReadOnlyList<TodoPriority> Priorities { get; } = new[] { TodoPriority.Low, TodoPriority.Medium, TodoPriority.High, TodoPriority.Urgent };

    private readonly IInteractionService _interactionService;

    public MainWindowViewModel(IInteractionService interactionService)
    {
        _interactionService = interactionService;
    }

    [RelayCommand]
    public async Task RequestAddTodo()
    {
        var vm = new TodoEditorViewModel(_interactionService)
        {
            Title = "Add New Todo",
            AcceptName = "Add"
        };

        var result = await _interactionService.RequestAsync(vm);
        if (result is not null)
            Todos.Add(result);
    }

    [RelayCommand]
    public void DeleteTodo(TodoViewModel todo)
    {
        Todos.Remove(todo);
    }
}
