using Avalonia.Animation;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using System;

namespace TransparentBrushTransitions;

/// <summary>
/// Improves brush color transitions to/from Avalonia's Transparent color (ie. Transparent white, #00FFFFFF)
/// Only works for ISolidColorBrush
/// </summary>
public class TransparentBrushTransition : InterpolatingTransitionBase<IBrush?>
{
    protected override IBrush? Interpolate(double progress, IBrush? from, IBrush? to)
    {
        if (from is null || to is null)
            return to ?? from;

        if (from is not ISolidColorBrush fromBrush || to is not ISolidColorBrush toBrush)
            throw new ArgumentException($"{nameof(TransparentBrushTransition)} bad input arguments");

        if (toBrush.Color == Colors.Transparent) // Interpolate only alpha, use from color channels
        {
            var aProgress = (byte)LerpHelpers.Lerp(fromBrush.Color.A, toBrush.Color.A, progress);
            var a = Math.Clamp(aProgress, (byte)0, (byte)255);

            return new ImmutableSolidColorBrush(Color.FromArgb(a, fromBrush.Color.R, fromBrush.Color.G, fromBrush.Color.B));
        }
        else if (fromBrush.Color == Colors.Transparent) // Interpolate only alpha, use to color channels
        {
            var aProgress = (byte)LerpHelpers.Lerp(fromBrush.Color.A, toBrush.Color.A, progress);
            var a = Math.Clamp(aProgress, (byte)0, (byte)255);

            return new ImmutableSolidColorBrush(Color.FromArgb(a, toBrush.Color.R, toBrush.Color.G, toBrush.Color.B));
        }
        else // interpolate all channels with same RGB strategy as BrushTransition
        {
            return new ImmutableSolidColorBrush(LerpHelpers.LerpColor(fromBrush.Color, toBrush.Color, progress));
        }
    }
}
