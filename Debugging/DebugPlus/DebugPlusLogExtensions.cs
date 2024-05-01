using Avalonia;
using Avalonia.Logging;
using Microsoft.Extensions.Logging;

namespace Monaco.Debugging;
public static class DebugPlusLogExtensions
{
    public static AppBuilder LogToDebugPlus(this AppBuilder builder, 
        LogEventLevel level = LogEventLevel.Warning,
        ILogger? logger = null,
        params string[] areas)
    {
        Logger.Sink = new DebugPlusLogSink(level, areas, logger);
        return builder;
    }
}
