using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Dialogs.Abstractions;
public abstract partial class DialogViewModel<TResult> : ObservableValidator, IRequestMediator<TResult>
{
    [ObservableProperty] protected TResult? _requestResult = default;
    [ObservableProperty] private string _title = "";

    private RelayCommand? _acceptCommand;
    public IRelayCommand AcceptCommand => _acceptCommand ??= new RelayCommand(Accept, CanAccept);

    private RelayCommand? _cancelCommand;
    public IRelayCommand CancelCommand => _cancelCommand ??= new RelayCommand(Cancel);

    public string AcceptName { get; init; } = "Ok";
    public string CancelName { get; init; } = "Cancel";

    /// <summary>
    /// Called when the user accepts the interaction
    /// Responsible for mapping an internal result into RequestResult
    /// </summary>
    protected abstract void Accept();

    protected virtual bool CanAccept() => true;

    /// <summary>
    /// Called when the user cancels an interaction
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("ObservablePropertyGenerator", "MVVMTK0034",
        Justification = "SetProperty must be used here to ensure PropertyChanged always fires")]
    protected virtual void Cancel()
    {
        SetProperty(ref _requestResult, default, false);
    }
}
