using Avalonia.Controls.Presenters;
using Avalonia.Media.TextFormatting;
using Avalonia.Media;
using Avalonia.Utilities;
using System.Collections.Generic;
using System;
using Avalonia;
using TextHighlighting.Utility;
using System.Runtime.CompilerServices;

namespace TextHighlighting.Controls;
public partial class HighlightingTextPresenter : TextPresenter
{
    public HighlightRangeCache Cache { get; } = new();

    /// <summary>
    /// Invalidates the highlight so the presenter updates
    /// </summary>
    public void InvalidateHighlight()
    {
        InvalidateTextLayout();
    }

    /// <summary>
    /// Creates the <see cref="TextLayout"/> used to render the text.
    /// </summary>
    /// <returns>A <see cref="TextLayout"/> object.</returns>
    protected override TextLayout CreateTextLayout()
    {
        TextLayout result;

        // Retrieves via reflection to avoid rewriting some code, could override measure though
        var _constraint = GetPrivateConstraint(this);

        var caretIndex = CaretIndex;
        var preeditText = PreeditText;
        var text = GetCombinedText(Text, caretIndex, preeditText);
        var typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
        var selectionStart = SelectionStart;
        var selectionEnd = SelectionEnd;
        var start = Math.Min(selectionStart, selectionEnd);
        var length = Math.Max(selectionStart, selectionEnd) - start;

        List<ValueSpan<TextRunProperties>> textStyleOverrides = [];
        IList<ValueSpan<TextRunProperties>> highlightOverrides = Array.Empty<ValueSpan<TextRunProperties>>();

        var foreground = Foreground;

        if (Cache.Count > 0)
        {
            highlightOverrides = new List<ValueSpan<TextRunProperties>>();
            var highlightProperties = new GenericTextRunProperties(typeface, FontFeatures, FontSize, foregroundBrush: Foreground, backgroundBrush: BackgroundHighlight);
            var defaultProperties = new GenericTextRunProperties(typeface, FontFeatures, FontSize, foregroundBrush: Foreground, backgroundBrush: Background);

            var range = Cache[0];
            highlightOverrides.Add(new(range.Index, range.Length, highlightProperties));
            int lastOffset = range.Index + range.Length;

            for (int i = 1; i < Cache.Count; i++)
            {
                range = Cache[i];
                var gapLength = range.Index - lastOffset;

                if (gapLength > 0)
                {
                    // textStyleOverrides do not fallback when the length ends, so manually inserting
                    // a fallback between highlighted words is required
                    highlightOverrides.Add(new(lastOffset, gapLength, defaultProperties));
                }

                highlightOverrides.Add(new(range.Index, range.Length, highlightProperties));
                lastOffset = range.Index + range.Length;
            }
        }

        if (!string.IsNullOrEmpty(preeditText))
        {
            var preeditHighlight = new ValueSpan<TextRunProperties>(caretIndex, preeditText.Length,
                    new GenericTextRunProperties(typeface, FontFeatures, FontSize,
                    foregroundBrush: foreground,
                    textDecorations: TextDecorations.Underline));

            textStyleOverrides = [ preeditHighlight ];
        }
        else
        {
            if (length > 0 && SelectionForegroundBrush != null)
            {
                textStyleOverrides =
                [
                    new ValueSpan<TextRunProperties>(start, length,
                    new GenericTextRunProperties(typeface, FontFeatures, FontSize,
                        foregroundBrush: SelectionForegroundBrush)),
                ];
            }
        }

        textStyleOverrides.AddRange(highlightOverrides);

        if (PasswordChar != default(char) && !RevealPassword)
        {
            result = CreateTextLayoutInternal(_constraint, new string(PasswordChar, text?.Length ?? 0), typeface,
                textStyleOverrides);
        }
        else
        {
            result = CreateTextLayoutInternal(_constraint, text, typeface, textStyleOverrides);
        }

        return result;
    }

    /// <summary>
    /// Creates the <see cref="TextLayout"/> used to render the text.
    /// </summary>
    /// <param name="constraint">The constraint of the text.</param>
    /// <param name="text">The text to format.</param>
    /// <param name="typeface"></param>
    /// <param name="textStyleOverrides"></param>
    /// <returns>A <see cref="TextLayout"/> object.</returns>
    private TextLayout CreateTextLayoutInternal(Size constraint, string? text, Typeface typeface,
        IReadOnlyList<ValueSpan<TextRunProperties>>? textStyleOverrides)
    {
        var foreground = Foreground;
        var maxWidth = MathUtilities.IsZero(constraint.Width) ? double.PositiveInfinity : constraint.Width;
        var maxHeight = MathUtilities.IsZero(constraint.Height) ? double.PositiveInfinity : constraint.Height;

        var textLayout = new TextLayout(text, typeface, FontFeatures, FontSize, foreground, TextAlignment,
            TextWrapping, maxWidth: maxWidth, maxHeight: maxHeight, textStyleOverrides: textStyleOverrides,
            flowDirection: FlowDirection, lineHeight: LineHeight, letterSpacing: LetterSpacing);

        return textLayout;
    }

    private static string? GetCombinedText(string? text, int caretIndex, string? preeditText)
    {
        if (string.IsNullOrEmpty(preeditText))
        {
            return text;
        }

        if (string.IsNullOrEmpty(text))
        {
            return preeditText;
        }

        var sb = StringBuilderCache.Acquire(text.Length + preeditText.Length);

        sb.Append(text.Substring(0, caretIndex));
        sb.Insert(caretIndex, preeditText);
        sb.Append(text.Substring(caretIndex));

        return StringBuilderCache.GetStringAndRelease(sb);
    }

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_constraint")]
    extern static ref Size GetPrivateConstraint(TextPresenter presenter);
}
