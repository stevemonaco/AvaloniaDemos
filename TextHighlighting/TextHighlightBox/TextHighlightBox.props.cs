using Avalonia.Media;
using Avalonia;

namespace TextHighlighting.Controls;
public partial class TextHighlightBox : IHighlighter
{
    public static readonly StyledProperty<string?> PatternProperty =
        AvaloniaProperty.Register<TextHighlightBlock, string?>(nameof(Pattern));

    public static readonly StyledProperty<bool> UseRegexProperty =
        AvaloniaProperty.Register<TextHighlightBlock, bool>(nameof(UseRegex));

    public static readonly StyledProperty<IBrush?> BackgroundHighlightProperty =
        AvaloniaProperty.Register<TextHighlightBlock, IBrush?>(nameof(BackgroundHighlight));

    public string? Pattern
    {
        get => GetValue(PatternProperty);
        set => SetValue(PatternProperty, value);
    }

    public bool UseRegex
    {
        get => GetValue(UseRegexProperty);
        set => SetValue(UseRegexProperty, value);
    }

    public IBrush? BackgroundHighlight
    {
        get => GetValue(BackgroundHighlightProperty);
        set => SetValue(BackgroundHighlightProperty, value);
    }
}