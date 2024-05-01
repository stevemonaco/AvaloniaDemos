using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.DependencyModel;
using Monaco.Debugging.Native;
using Avalonia.Platform;
using Avalonia.VisualTree;
using HarfBuzzSharp;
using Avalonia;
using Avalonia.LogicalTree;

namespace Monaco.Debugging;

public record AssetRuntimeInfo(Uri Uri, long Length);
public record AssemblyRuntimeInfo(string Name, Uri? Uri);
public record FontNameIdInfo(string Id, string Name);

public static class DebugPlus
{
    public static Visual? FindVisualByHashCode(Visual rootVisual, int hashCode)
    {
        return rootVisual.GetSelfAndVisualDescendants().FirstOrDefault(x => x.GetHashCode() == hashCode);
    }

    public static ILogical? FindLogicalByHashCode(Visual rootVisual, int hashCode)
    {
        return rootVisual.GetSelfAndLogicalDescendants().FirstOrDefault(x => x.GetHashCode() == hashCode);
    }

    /// <summary>
    /// Gets info for all assets from loaded assemblies
    /// </summary>
    /// <returns></returns>
    public static List<AssetRuntimeInfo> GetAllAssetInfo()
    {
        var assets = new List<AssetRuntimeInfo>();
        var assetUris = new List<Uri>();

        foreach (var assemblyInfo in GetAllAssemblyInfo(true))
        {
            try
            {
                var assemblyUri = new Uri($"avares://{assemblyInfo.Name}");

                foreach (var assetUri in AssetLoader.GetAssets(assemblyUri, null))
                {
                    assetUris.Add(assetUri);
                }
            }
            catch (Exception) { }
        }

        // Fetch the lengths of each Asset
        foreach (var uri in assetUris)
        {
            var asset = AssetLoader.Open(uri);
            using var reader = new StreamReader(asset);
            var info = new AssetRuntimeInfo(uri, asset.Length);
            
            assets.Add(info);
        }

        return assets;
    }

    /// <summary>
    /// Gets info for all loaded assemblies
    /// </summary>
    /// <param name="requireValidUri">Requires a valid Uri to be formed. Primarily affects native dependencies.</param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException">Cannot be used in single file publishes</exception>
    public static List<AssemblyRuntimeInfo> GetAllAssemblyInfo(bool requireValidUri = false)
    {
        if (DependencyContext.Default is null)
            throw new NotSupportedException($"DependencyContext.Default is unavailable. Possibly caused by single file publish.");

        var runtimeLibraries = DependencyContext.Default.RuntimeLibraries;
        var assemblies = new List<AssemblyRuntimeInfo>();

        foreach (var library in runtimeLibraries)
        {
            try
            {
                var assembly = Assembly.Load(new AssemblyName(library.Name));
                var info = new AssemblyRuntimeInfo(assembly.GetName().Name ?? "Unspecified", new Uri(assembly.Location));

                assemblies.Add(info);
            }
            catch (FileNotFoundException)
            {
                if (!requireValidUri)
                    assemblies.Add(new(library.Name, null));
            }
        }

        return assemblies;
    }

    /// <summary>
    /// Gets all of the Name Ids from a Font Asset Uri
    /// Only gets for the first Face and uses "en" as the requested language
    /// </summary>
    /// <param name="fontUri">Asset Uri to load as font</param>
    /// <returns></returns>
    public static unsafe List<FontNameIdInfo> GetNameIdsFromFontUri(Uri? fontUri)
    {
        if (fontUri is null || !AssetLoader.Exists(fontUri))
            return [];

        var results = new List<FontNameIdInfo>();

        using var assetStream = AssetLoader.Open(fontUri);
        using var stream = new MemoryStream();
        assetStream.CopyTo(stream);
        stream.Position = 0;

        var raw = stream.GetBuffer();

        fixed (byte* pRaw = raw)
        {
            var blob = new Blob((nint)pRaw, raw.Length, MemoryMode.ReadOnly);
            Console.WriteLine($"Blob Face Count: {blob.FaceCount}");

            var face = new Face(blob, 0);
            var lang = HBNative.hb_language_from_string("en", Encoding.UTF8.GetByteCount("en"));

            foreach (var id in Enum.GetValues<OpenTypeNameId>().Except([OpenTypeNameId.Invalid]))
            {
                var name = GetName(face.Handle, id, lang);
                var info = new FontNameIdInfo(id.ToString(), name ?? "Unspecified");
                results.Add(info);
            }
        }

        return results;

        static string? GetName(nint FaceHandle, OpenTypeNameId id, nint Lang)
        {
            var nameBuf = new char[512];
            uint nameSize = 512;

            fixed (char* pNameBuf = nameBuf)
            {
                HBNative.hb_ot_name_get_utf8(FaceHandle, id, Lang, &nameSize, pNameBuf);
                return Marshal.PtrToStringUTF8((nint)pNameBuf);
            }
        }
    }
}
