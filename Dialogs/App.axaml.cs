using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using Dialogs.Abstractions;
using Dialogs.Services;
using Dialogs.ViewModels;
using Dialogs.Views;
using Jot;
using Microsoft.Extensions.DependencyInjection;

namespace Dialogs;
public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            BindingPlugins.DataValidators.RemoveAt(0);

            var services = new ServiceCollection();
            services.AddSingleton<IInteractionService, InteractionService>();
            services.AddSingleton<ITodoService, TodoService>();
            services.AddSingleton<IFileRequestService, FileRequestService>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<PresentationFactory>();
            services.AddSingleton<Tracker>();

            services.AddTransient<TodoEditorViewModel>();

            var provider = services.BuildServiceProvider();
            Ioc.Default.ConfigureServices(provider);

            var _mainViewModel = provider.GetService<MainWindowViewModel>();
            var _mainView = provider.GetService<MainWindow>();
            _mainView!.DataContext = _mainViewModel;

            var viewLocator = new ViewLocator(provider.GetService<PresentationFactory>()!);
            DataTemplates.Add(viewLocator);

            desktop.MainWindow = _mainView;
        }

        base.OnFrameworkInitializationCompleted();
    }
}