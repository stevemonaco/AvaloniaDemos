using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dialogs.Abstractions;
using Dialogs.Mappers;
using Dialogs.Models;
using Dialogs.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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

    private readonly ITodoService _todoService;
    private readonly IInteractionService _interactionService;
    private readonly IFileRequestService _fileRequestService;

    public MainWindowViewModel(ITodoService todoService, IInteractionService interactionService, IFileRequestService fileRequestService)
    {
        _todoService = todoService;
        _interactionService = interactionService;
        _fileRequestService = fileRequestService;
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

    [RelayCommand]
    public async Task RequestLoadTodos()
    {
        var filename = await _fileRequestService.RequestLoadTodoFileName();

        if (filename is null)
            return;

        var content = await File.ReadAllTextAsync(filename);
        var todos = _todoService.DeserializeTodos(content);

        if (todos is null)
            return;

        Todos = new(todos.Select(x => x.ToViewModel()));
    }

    [RelayCommand]
    public async Task RequestSaveTodos()
    {
        var filename = await _fileRequestService.RequestSaveTodoFileName();

        if (filename is null)
            return;

        var content = _todoService.SerializeTodos(Todos.Select(x => x.ToModel()));
        await File.WriteAllTextAsync(filename, content);
    }
}
