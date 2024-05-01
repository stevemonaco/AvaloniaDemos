using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Avalonia;
using Avalonia.Logging;
using Avalonia.Utilities;
using Avalonia.VisualTree;
using Microsoft.Extensions.Logging;

namespace Monaco.Debugging;

/// <summary>
/// Logs Avalonia diagnostic messages using the provided Logger or to Trace
/// Adds Visual Tree and Hash information when formatting messages related to a Control (StyledElement)
/// </summary>
public class DebugPlusLogSink : ILogSink
{
    public ILogger? Logger { get; set; }

    private readonly LogEventLevel _level;
    private readonly IList<string>? _areas;

    public DebugPlusLogSink(
        LogEventLevel minimumLevel,
        IList<string>? areas = null,
        ILogger? logger = null)
    {
        _level = minimumLevel;
        _areas = areas?.Count > 0 ? areas : null;
        Logger = logger;
    }

    public bool IsEnabled(LogEventLevel level, string area)
    {
        return level >= _level && (_areas?.Contains(area) ?? true);
    }

    public void Log(LogEventLevel level, string area, object? source, string messageTemplate)
    {
        if (IsEnabled(level, area))
        {
            var message = Format<object, object, object>(area, messageTemplate, source, null);

            if (Logger is null)
                Trace.WriteLine(message);
            else
                Logger.Log(ToLogLevel(level), message);
        }
    }

    public void Log(LogEventLevel level, string area, object? source, string messageTemplate, params object?[] propertyValues)
    {
        if (IsEnabled(level, area))
        {
            var message = Format(area, messageTemplate, source, propertyValues);

            if (Logger is null)
                Trace.WriteLine(message);
            else
                Logger.Log(ToLogLevel(level), message);
        }
    }

    private static string Format<T0, T1, T2>(
        string area,
        string template,
        object? source,
        object?[]? values)
    {
        var result = StringBuilderCache.Acquire(template.Length);
        var r = new CharacterReader(template.AsSpan());
        var i = 0;

        result.Append('[');
        result.Append(area);
        result.Append("] ");

        while (!r.End)
        {
            var c = r.Take();

            if (c != '{')
            {
                result.Append(c);
            }
            else
            {
                if (r.Peek != '{')
                {
                    result.Append('\'');
                    result.Append(values?[i++]);
                    result.Append('\'');
                    r.TakeUntil('}');
                    r.Take();
                }
                else
                {
                    result.Append('{');
                    r.Take();
                }
            }
        }

        FormatSource(source, result);
        return StringBuilderCache.GetStringAndRelease(result);
    }

    private static string Format(
        string area,
        string template,
        object? source,
        object?[] v)
    {
        var result = StringBuilderCache.Acquire(template.Length);
        var r = new CharacterReader(template.AsSpan());
        var i = 0;

        result.Append('[');
        result.Append(area);
        result.Append(']');

        while (!r.End)
        {
            var c = r.Take();

            if (c != '{')
            {
                result.Append(c);
            }
            else
            {
                if (r.Peek != '{')
                {
                    result.Append('\'');
                    result.Append(i < v.Length ? v[i++] : null);
                    result.Append('\'');
                    r.TakeUntil('}');
                    r.Take();
                }
                else
                {
                    result.Append('{');
                    r.Take();
                }
            }
        }

        FormatSource(source, result);
        return StringBuilderCache.GetStringAndRelease(result);
    }

    private static void FormatSource(object? source, StringBuilder result)
    {
        if (source is null)
            return;

        result.Append($" ({source.GetType().Name}");

        if (source is Visual v)
        {
            var tree = FormatVisualTree(v);
            result.Append($" #{source.GetHashCode()}");
            if (!string.IsNullOrEmpty(v.Name))
                result.Append($" #{v.Name}");

            result.Append(" Visual Tree: ");
            result.Append(tree);
        }
        else if (source is StyledElement se)
        {
            result.Append($" #{source.GetHashCode()}");
            if (!string.IsNullOrEmpty(se.Name))
                result.Append($" #{se.Name}");
        }

        result.Append(')');
    }

    private static string FormatVisualTree(Visual visual)
    {
        return string.Join(" -> ", visual.GetSelfAndVisualAncestors().Reverse().Select(x => x.GetType().Name));
    }

    private static LogLevel ToLogLevel(LogEventLevel level) => level switch
    {
        LogEventLevel.Verbose => LogLevel.Trace,
        LogEventLevel.Debug => LogLevel.Debug,
        LogEventLevel.Information => LogLevel.Information,
        LogEventLevel.Warning => LogLevel.Warning,
        LogEventLevel.Error => LogLevel.Error,
        LogEventLevel.Fatal => LogLevel.Critical,
        _ => LogLevel.None
    };
}