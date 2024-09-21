using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Dialogs;

/// <summary>
/// Creates instances of ViewModels and Views by type, along with type-based ViewModel -> View resolution
/// </summary>
public class PresentationFactory
{
    private Dictionary<Type, Func<Control>> _locatorFactory = new();

    public Control ResolveView<T>() => ResolveView(typeof(T));

    public Control ResolveView(INotifyPropertyChanged viewModel) => ResolveView(viewModel.GetType());

    private Control ResolveView(Type type)
    {
        if (_locatorFactory.TryGetValue(type, out var factory)) // Resolve by registration
        {
            return factory.Invoke();
        }
        else // Resolve by convention
        {
            var name = type.FullName!.Replace("ViewModel", "View");
            var viewType = Type.GetType(name);

            if (viewType != null)
            {
                return (Control)Activator.CreateInstance(viewType)!;
            }
        }

        throw new InvalidOperationException($"Could not resolve the View for '{type}'");
    }

    public TViewModel CreateViewModel<TViewModel>() where TViewModel : ObservableObject
    {
        return Ioc.Default.GetService<TViewModel>()!;
    }

    public Control CreateView<TView>() where TView : Control
    {
        return Activator.CreateInstance<TView>()!;
    }

    public void RegisterViewFactory<TViewModel, TView>()
        where TViewModel : class
        where TView : Control
        => _locatorFactory.Add(typeof(TViewModel), CreateView<TView>);
}
