using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace TextHighlighting.Controls;

public partial class TextHighlightBox : TextBox
{
    private HighlightingTextPresenter? _textPresenter;
    private HighlightRangeCache? _cache;

    static TextHighlightBox()
    {
        AffectsRender<TextHighlightBox>(PatternProperty, UseRegexProperty, BackgroundHighlightProperty);

        TextProperty.Changed.AddClassHandler<TextHighlightBox>((x, e) => x.InvalidateHighlight());
        PatternProperty.Changed.AddClassHandler<TextHighlightBox>((x, e) => x.InvalidateHighlight());
        UseRegexProperty.Changed.AddClassHandler<TextHighlightBox>((x, e) => x.InvalidateHighlight());
    }

    protected void InvalidateHighlight()
    {
        if (_textPresenter is null)
            return;

        BuildHighlightRanges();
        _textPresenter.InvalidateHighlight();
    }

    protected void BuildHighlightRanges()
    {
        if (_cache is null)
            return;

        _cache.Clear();

        if (string.IsNullOrEmpty(Text) || string.IsNullOrEmpty(Pattern))
            return;

        _cache.BuildHighlightRanges(Text, Pattern, UseRegex, 0);
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _textPresenter = e.NameScope.Get<HighlightingTextPresenter>("PART_TextPresenter")!;

        _cache = _textPresenter.Cache;
        BuildHighlightRanges();
    }
}
