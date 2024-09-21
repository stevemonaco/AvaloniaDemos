using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System.Collections.Generic;
using System.Linq;

namespace Dialogs.Services;

/// <summary>
/// View-neutral interface to request a user to select file(s)
/// This is application-specific for each type of request
/// </summary>
public interface IFileRequestService
{
    Task<string?> RequestLoadTodoFileName();
    Task<string?> RequestSaveTodoFileName();
}

public class FileRequestService : IFileRequestService
{
    public async Task<string?> RequestLoadTodoFileName()
    {
        if (GetStorageProvider() is not { } storageProvider)
            return null;

        var folder = await storageProvider.TryGetFolderFromPathAsync("_data");

        var options = new FilePickerOpenOptions()
        {
            FileTypeFilter = new List<FilePickerFileType>()
            {
                new FilePickerFileType("Todos File")
                {
                    Patterns = ["*.json"],
                    AppleUniformTypeIdentifiers = ["public.json"],
                    MimeTypes = ["application/json"]
                }
            },
            SuggestedStartLocation = folder,
            Title = "Load Todo JSON File"
        };

        var pickerResult = await storageProvider.OpenFilePickerAsync(options);
        return pickerResult?.FirstOrDefault()?.TryGetLocalPath();
    }

    public async Task<string?> RequestSaveTodoFileName()
    {
        if (GetStorageProvider() is not { } storageProvider)
            return null;

        var folder = await storageProvider.TryGetFolderFromPathAsync("_data");

        var options = new FilePickerSaveOptions()
        {
            SuggestedFileName = ".json",
            DefaultExtension = "json",
            SuggestedStartLocation = folder,
            Title = "Save Todo JSON File"
        };

        var pickerResult = await storageProvider.SaveFilePickerAsync(options);
        return pickerResult?.TryGetLocalPath();
    }

    private static IStorageProvider? GetStorageProvider()
    {
        var lifetime = Avalonia.Application.Current!.ApplicationLifetime;

        var root = lifetime switch
        {
            IClassicDesktopStyleApplicationLifetime desktop => desktop.MainWindow,
            ISingleViewApplicationLifetime single => single.MainView,
            _ => null
        };

        if (root is null)
            return null;

        return TopLevel.GetTopLevel(root)?.StorageProvider;
    }
}