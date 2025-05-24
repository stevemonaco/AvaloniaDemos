using CommunityToolkit.Mvvm.ComponentModel;

namespace TabStripViewCaching.ViewModels;
public partial class PersonViewModel : TabViewModel
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayName))]
    private int _id;

    [ObservableProperty] private string? _name;
    [ObservableProperty] private int _age;
    [ObservableProperty] private string? _hobby;

    public override string? DisplayName => $"Person ({Id})";
    private static int _idCounter = 1;

    public PersonViewModel()
    {
        Id = _idCounter++;
    }
}
