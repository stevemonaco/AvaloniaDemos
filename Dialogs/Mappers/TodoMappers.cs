using Dialogs.Models;
using Dialogs.ViewModels;

namespace Dialogs.Mappers;
public static class TodoMappers
{
    public static TodoModel ToModel(this TodoViewModel vm) =>
        new TodoModel(vm.Activity, vm.IsCompleted, vm.Priority);

    public static TodoViewModel ToViewModel(this TodoModel model) =>
        new TodoViewModel()
        {
            Activity = model.Activity,
            IsCompleted = model.IsCompleted,
            Priority = model.Priority
        };
}
