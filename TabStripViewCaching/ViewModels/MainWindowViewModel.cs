using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace TabStripViewCaching.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public ObservableCollection<TabViewModel> TabControlItems { get; } = new()
    {
        new PersonViewModel(),
        new PersonViewModel(),
        new PersonViewModel()
    };

    public ObservableCollection<TabViewModel> TabStripItems { get; } = new()
    {
        new PersonViewModel(),
        new PersonViewModel(),
        new PersonViewModel()
    };


    [RelayCommand]
    private void AddPersonToTabControlItems()
    {
        TabControlItems.Add(new PersonViewModel());
    }

    [RelayCommand]
    private void AddPersonToTabStripItems()
    {
        TabStripItems.Add(new PersonViewModel());
    }
}
