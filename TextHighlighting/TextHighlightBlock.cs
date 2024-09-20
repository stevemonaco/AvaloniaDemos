using Avalonia.Media;
using Avalonia;
using Avalonia.Controls;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;

namespace TextHighlighting.Controls;

public partial class TextHighlightBlock : SelectableTextBlock
{
    private List<(int Index, int Length)> _highlightRangeCache = new();

    static TextHighlightBlock()
    {
        AffectsRender<TextHighlightBlock>(PatternProperty, UseRegexProperty, HighlightProperty);

        TextProperty.Changed.AddClassHandler<TextHighlightBlock>((x, e) => x.BuildHighlightRanges());
        PatternProperty.Changed.AddClassHandler<TextHighlightBlock>((x, e) => x.BuildHighlightRanges());
    }

    protected void BuildHighlightRanges()
    {
        _highlightRangeCache.Clear();

        if (string.IsNullOrEmpty(Text) || string.IsNullOrEmpty(Pattern))
            return;

        if (UseRegex)
            BuildRegexRanges();
        else
            BuildPlainTextRanges();

        void BuildRegexRanges()
        {
            MatchCollection? matches = null;

            try
            {
                matches = Regex.Matches(Text, Pattern);
            }
            catch (Exception)
            {
            }

            if (matches is null)
                return;

            foreach (Match match in matches)
            {
                _highlightRangeCache.Add((match.Index, match.ValueSpan.Length));
            }
        }

        void BuildPlainTextRanges()
        {
            int startIndex = 0;

            while ((startIndex = Text.IndexOf(Pattern, startIndex)) != -1)
            {
                _highlightRangeCache.Add((startIndex, Pattern.Length));
                startIndex += Pattern.Length;
            }
        }
    }

    protected void RenderHighlightRanges(DrawingContext context, Point origin)
    {
        if (_highlightRangeCache is null || Highlight is null)
            return;

        foreach (var range in _highlightRangeCache)
        {
            var rects = TextLayout.HitTestTextRange(range.Index, range.Length);

            using (context.PushTransform(Matrix.CreateTranslation(origin)))
            {
                foreach (var rect in rects)
                {
                    context.FillRectangle(Highlight, PixelRect.FromRect(rect, 1).ToRect(1));
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
