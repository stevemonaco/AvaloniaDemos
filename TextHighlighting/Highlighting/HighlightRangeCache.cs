using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TextHighlighting.Controls;

/// <summary>
/// Performs pattern matching on text and caches the locations that match
/// </summary>
public class HighlightRangeCache : List<(int Index, int Length)>
{
    /// <summary>
    /// Builds the highlight ranges for the given text and pattern
    /// </summary>
    /// <param name="text">Text slice to be searched</param>
    /// <param name="pattern">Pattern to find</param>
    /// <param name="useRegex">True if Pattern is Regex, false if plain text</param>
    /// <param name="startingOffset">Starting offset within the original text that the text slice comes from, relative to index 0 in the text control</param>
    public void BuildHighlightRanges(ReadOnlySpan<char> text, string pattern, bool useRegex, int startingOffset = 0)
    {
        Clear();

        if (text.IsEmpty || string.IsNullOrEmpty(pattern))
            return;

        if (useRegex)
            BuildRegexRanges(text, pattern, startingOffset);
        else
            BuildPlainTextRanges(text, pattern, startingOffset);
    }

    private void BuildRegexRanges(ReadOnlySpan<char> text, string pattern, int startingOffset)
    {
        Regex.ValueMatchEnumerator matches = default;

        try
        {
            matches = Regex.EnumerateMatches(text, pattern);
        }
        catch (Exception)
        {
        }

        foreach (var match in matches)
        {
            Add((match.Index + startingOffset, match.Length));
        }
    }

    private void BuildPlainTextRanges(ReadOnlySpan<char> text, string pattern, int startingOffset)
    {
        int sliceIndex = 0;
        int foundIndex = 0;

        var slice = text;

        while ((foundIndex = slice.IndexOf(pattern)) != -1)
        {
            Add((startingOffset + sliceIndex + foundIndex, pattern.Length));
            slice = slice.Slice(foundIndex + pattern.Length);
            sliceIndex += foundIndex + pattern.Length;
        }
    }
}
