﻿using Avalonia.Media;
using System;

namespace TransparentBrushTransitions;

/// <summary>
/// Helps perform linear interpolations on types
/// Methods taken from Avalonia source code
/// </summary>
public static class LerpHelpers
{
    public static double Lerp(double a, double b, double p) => p * (b - a) + a;

    public static Color LerpColor(Color oldValue, Color newValue, double progress)
    {
        // normalize sRGB values.
        var oldA = oldValue.A / 255d;
        var oldR = oldValue.R / 255d;
        var oldG = oldValue.G / 255d;
        var oldB = oldValue.B / 255d;

        var newA = newValue.A / 255d;
        var newR = newValue.R / 255d;
        var newG = newValue.G / 255d;
        var newB = newValue.B / 255d;

        // convert from sRGB to linear
        oldR = EOCF_sRGB(oldR);
        oldG = EOCF_sRGB(oldG);
        oldB = EOCF_sRGB(oldB);

        newR = EOCF_sRGB(newR);
        newG = EOCF_sRGB(newG);
        newB = EOCF_sRGB(newB);

        // compute the interpolated color in linear space
        var a = oldA + progress * (newA - oldA);
        var r = oldR + progress * (newR - oldR);
        var g = oldG + progress * (newG - oldG);
        var b = oldB + progress * (newB - oldB);

        // convert back to sRGB in the [0..255] range
        a *= 255d;
        r = OECF_sRGB(r) * 255d;
        g = OECF_sRGB(g) * 255d;
        b = OECF_sRGB(b) * 255d;

        return new Color((byte)Math.Round(a), (byte)Math.Round(r), (byte)Math.Round(g), (byte)Math.Round(b));
    }

    /// <summary>
    /// Opto-electronic conversion function for the sRGB color space.
    /// Takes a gamma-encoded sRGB value and converts it to a linear sRGB value.
    /// </summary>
    private static double OECF_sRGB(double linear)
    {
        // IEC 61966-2-1:1999
        return linear <= 0.0031308d ? linear * 12.92d : (double)(Math.Pow(linear, 1.0d / 2.4d) * 1.055d - 0.055d);
    }

    /// <summary>
    /// Electro-optical conversion function for the sRGB color space.
    /// Takes a linear sRGB value and converts it to a gamma-encoded sRGB value.
    /// </summary>
    private static double EOCF_sRGB(double srgb)
    {
        // IEC 61966-2-1:1999
        return srgb <= 0.04045d ? srgb / 12.92d : (double)Math.Pow((srgb + 0.055d) / 1.055d, 2.4d);
    }
}
