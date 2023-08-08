namespace Dialogs.Abstractions;

/// <summary>
/// View-neutral interface to request a user to select file(s)
/// </summary>
public interface IAsyncFileRequestService
{
    Task<Uri?> RequestProjectFileName();
    Task<Uri?> RequestNewProjectFileName();
    Task<Uri?> RequestExistingDataFileName();
    Task<Uri?> RequestExportArrangerFileName(string defaultName);
    Task<Uri?> RequestImportArrangerFileName();
}