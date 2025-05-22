using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;
using System;
using System.Collections.Generic;

namespace TabStripViewCaching;

/// <summary>
/// Maps a type to a view-building factory. Avoids recycling that <see cref="DataTemplate"/> has. />
/// </summary>
public class TypedTemplateSelector : IDataTemplate
{
    [Content]
    public Dictionary<Type, IDataTemplate> AvailableTemplates { get; } = new Dictionary<Type, IDataTemplate>();

    public Control Build(object? param)
    {
        var key = param?.GetType();

        if (key is null)
        {
            throw new ArgumentNullException(nameof(param));
        }

        return AvailableTemplates[key].Build(param)!;
    }

    public bool Match(object? data)
    {
        var key = data?.GetType();

        if (key is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        return AvailableTemplates.ContainsKey(key);
    }
}