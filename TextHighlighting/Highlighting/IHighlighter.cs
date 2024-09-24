using Avalonia.Media;

namespace TextHighlighting.Controls;
public interface IHighlighter
{
    string? Pattern { get; set; }
    bool UseRegex { get; set; }
    IBrush? BackgroundHighlight { get; set; }
}
