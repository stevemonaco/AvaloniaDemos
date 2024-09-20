using Avalonia.Media;
using Avalonia;

namespace TextHighlighting.Controls;
public partial class TextHighlightBlock
{
    public static readonly StyledProperty<string?> PatternProperty =
        AvaloniaProperty.Register<TextHighlightBlock, string?>(nameof(Pattern));

    public static readonly StyledProperty<bool> UseRegexProperty =
        AvaloniaProperty.Register<TextHighlightBlock, bool>(nameof(UseRegex));

    public static readonly StyledProperty<IBrush?> HighlightProperty =
        AvaloniaProperty.Register<TextHighlightBlock, IBrush?>(nameof(Highlight));

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

    public IBrush? Highlight
    {
        get => GetValue(HighlightProperty);
        set => SetValue(HighlightProperty, value);
    }
}
