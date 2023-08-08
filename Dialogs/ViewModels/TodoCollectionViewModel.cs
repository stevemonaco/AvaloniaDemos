using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dialogs.ViewModels;
public partial class TodoCollectionViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<TodoViewModel> _todos = new();


}
