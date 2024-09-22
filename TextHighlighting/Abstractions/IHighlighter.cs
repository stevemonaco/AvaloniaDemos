using Avalonia.Media;

namespace TextHighlighting.Abstractions;
public interface IHighlighter
{
    string? Pattern { get; set; }
    bool UseRegex { get; set; }
    IBrush? Highlight { get; set; }
}
