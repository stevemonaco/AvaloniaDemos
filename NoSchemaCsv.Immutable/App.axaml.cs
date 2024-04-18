using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using NoSchemaCsv.Immutable.Services;
using NoSchemaCsv.Immutable.ViewModels;
using NoSchemaCsv.Immutable.Views;

namespace NoSchemaCsv.Immutable;
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

            // Setup DI
            var services = new ServiceCollection();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddTransient<CsvService>();
            services.AddTransient<CsvGeneratorService>();

            var provider = services.BuildServiceProvider();
            Ioc.Default.ConfigureServices(provider);

            var _mainViewModel = provider.GetService<MainWindowViewModel>();
            var _mainView = provider.GetService<MainWindow>();
            _mainView!.DataContext = _mainViewModel;

            desktop.MainWindow = _mainView;
        }

        base.OnFrameworkInitializationCompleted();
    }
}