using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Dialogs.ViewModels;
public partial class TodoCollectionViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<TodoViewModel> _todos = new();


}
