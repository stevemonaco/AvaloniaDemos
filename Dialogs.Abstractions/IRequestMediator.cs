using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;

namespace Dialogs.Abstractions;

/// <summary>
/// Abstraction representing an extended interaction with the user, typically a dialog
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface IRequestMediator<TResult> : INotifyPropertyChanged
{
    string Title { get; }
    string AcceptName { get; }
    string CancelName { get; }
    TResult? RequestResult { get; set; }

    IRelayCommand AcceptCommand { get; }
    IRelayCommand CancelCommand { get; }
}
