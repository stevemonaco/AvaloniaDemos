using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Dialogs;
public class ViewLocator : IDataTemplate
{
    private readonly PresentationFactory _presentationFactory;

    public ViewLocator(PresentationFactory presentationFactory)
    {
        _presentationFactory = presentationFactory;
    }

    public Control Build(object? data)
    {
        if (data is null)
            return new TextBlock { Text = $"{nameof(data)} was null" };

        if (data is not ObservableObject viewModel)
            return new TextBlock { Text = $"{nameof(data)} is not a ViewModel" };

        return _presentationFactory.ResolveView(viewModel);
    }

    public bool Match(object? data) => data is ObservableObject;
}