using CommunityToolkit.Mvvm.ComponentModel;

namespace TabStripViewCaching.ViewModels;
public abstract class TabViewModel : ObservableObject
{
    public abstract string? DisplayName { get; }
}
