using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Monaco.Debugging;
using System.Collections.ObjectModel;
using System;
using System.Linq;
using System.IO;

namespace Debugging.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<string> _display = new();
    [ObservableProperty] private bool _filterResourceXamlInfo = true;
    [ObservableProperty] private string _fontUri = "";

    [RelayCommand]
    public void ListAssemblies()
    {
        Display.Clear();

        foreach (var assembly in DebugPlus.GetAllAssemblyInfo())
        {
            Display.Add($"Assembly Name: {assembly.Name}, URI: {assembly.Uri?.AbsoluteUri ?? "Unavailable"}");
        }
    }

    [RelayCommand]
    public void ListAssets()
    {
        Display.Clear();

        var infos = DebugPlus.GetAllAssetInfo()
            .Where(x => FilterResourceXamlInfo && !x.Uri.AbsolutePath.Contains("!AvaloniaResourceXamlInfo"));

        foreach (var info in infos)
        {
            Display.Add($"Asset URI: {info.Uri} | Size: {info.Length}");
        }
    }

    [RelayCommand]
    public void ListFontIdNamesCommand()
    {
        if (string.IsNullOrWhiteSpace(FontUri))
            return;

        Display.Clear();

        var uri = new Uri(FontUri);
        var infos = DebugPlus.GetNameIdsFromFontUri(uri);
        var fontName = infos.FirstOrDefault(x => x.Id == "FontFamily")?.Name;
        var typographicName = infos.FirstOrDefault(x => x.Id == "TypographicFamily")?.Name;

        // Poor code for building an avares:// scheme without the font file name
        var fontFolder = Path.GetDirectoryName(FontUri).Replace('\\', '/').Replace("avares:/", "avares://");

        string? standardFontName = string.IsNullOrEmpty(typographicName) ? fontName : typographicName;
        string resourceFontName = $"{fontFolder}#{standardFontName}";

        foreach (var info in infos)
        {
            Display.Add($"{info.Id}: {info.Name}");
        }

        Display.Add($@"!Avalonia Resource: {resourceFontName}");
        Display.Add($@"!AXAML Snippet: <FontFamily x:Key="""">{resourceFontName}</FontFamily>");
    }
}
