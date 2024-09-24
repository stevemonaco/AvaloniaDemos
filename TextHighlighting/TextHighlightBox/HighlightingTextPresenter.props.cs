using Avalonia.Media;
using Avalonia;

namespace TextHighlighting.Controls;
public partial class HighlightingTextPresenter
{
    public static readonly StyledProperty<IBrush?> BackgroundHighlightProperty =
        AvaloniaProperty.Register<TextHighlightBlock, IBrush?>(nameof(BackgroundHighlight));

    public IBrush? BackgroundHighlight
    {
        get => GetValue(BackgroundHighlightProperty);
        set => SetValue(BackgroundHighlightProperty, value);
    }
}
