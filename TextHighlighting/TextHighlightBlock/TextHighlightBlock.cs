using Avalonia.Media;
using Avalonia;
using Avalonia.Controls;

namespace TextHighlighting.Controls;

public partial class TextHighlightBlock : SelectableTextBlock
{
    private HighlightRangeCache _cache = new();

    static TextHighlightBlock()
    {
        AffectsRender<TextHighlightBlock>(PatternProperty, UseRegexProperty, BackgroundHighlightProperty);

        TextProperty.Changed.AddClassHandler<TextHighlightBlock>((x, e) => x.BuildHighlightRanges());
        PatternProperty.Changed.AddClassHandler<TextHighlightBlock>((x, e) => x.BuildHighlightRanges());
        UseRegexProperty.Changed.AddClassHandler<TextHighlightBlock>((x, e) => x.BuildHighlightRanges());
    }

    protected void BuildHighlightRanges()
    {
        _cache.Clear();

        if (string.IsNullOrEmpty(Text) || string.IsNullOrEmpty(Pattern))
            return;

        _cache.BuildHighlightRanges(Text, Pattern, UseRegex, 0);
    }

    protected void RenderHighlightRanges(DrawingContext context, Point origin)
    {
        if (BackgroundHighlight is null)
            return;

        foreach (var range in _cache)
        {
            var rects = TextLayout.HitTestTextRange(range.Index, range.Length);

            using (context.PushTransform(Matrix.CreateTranslation(origin)))
            {
                foreach (var rect in rects)
                {
                    context.FillRectangle(BackgroundHighlight, PixelRect.FromRect(rect, 1).ToRect(1));
                }
            }
        }
    }

    protected override void RenderTextLayout(DrawingContext context, Point origin)
    {
        RenderHighlightRanges(context, origin);
        base.RenderTextLayout(context, origin);
    }
}
