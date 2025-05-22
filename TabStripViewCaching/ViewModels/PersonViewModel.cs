using CommunityToolkit.Mvvm.ComponentModel;

namespace TabStripViewCaching.ViewModels;
public partial class PersonViewModel : TabViewModel
{
    [ObservableProperty] private string? _name;
    [ObservableProperty] private int _age;
    [ObservableProperty] private string? _hobby;

    public override string? DisplayName { get; set; } = "Person";
}
