// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace RealTimeBitmapAdapter.Rng;

/// <summary>
/// Provides an implementation of the xoshiro256** algorithm. This implementation is used
/// on 64-bit when no seed is specified and an instance of the base Random class is constructed.
/// As such, we are free to implement however we see fit, without back compat concerns around
/// the sequence of numbers generated or what methods call what other methods.
/// </summary>
/// <remarks>
/// Trimmed down implementation from .NET (XoshiroImpl) with less indirection through Random
/// https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Random.Xoshiro128StarStarImpl.cs
/// </remarks>
internal sealed class XoshiroRandom
{
    // NextUInt64 is based on the algorithm from http://prng.di.unimi.it/xoshiro256starstar.c:
    //
    //     Written in 2018 by David Blackman and Sebastiano Vigna (vigna@acm.org)
    //
    //     To the extent possible under law, the author has dedicated all copyright
    //     and related and neighboring rights to this software to the public domain
    //     worldwide. This software is distributed without any warranty.
    //
    //     See <http://creativecommons.org/publicdomain/zero/1.0/>.

    private ulong _s0, _s1, _s2, _s3;

    public XoshiroRandom()
    {
        //ulong* ptr = stackalloc ulong[4];
        do
        {
            //Interop.GetRandomBytes((byte*)ptr, 4 * sizeof(ulong));

            // Less interesting than the .NET implementation above, but good enough since we don't have access to Interop
            _s0 = (ulong)Random.Shared.NextInt64();
            _s1 = (ulong)Random.Shared.NextInt64();
            _s2 = (ulong)Random.Shared.NextInt64();
            _s3 = (ulong)Random.Shared.NextInt64();
        }
        while ((_s0 | _s1 | _s2 | _s3) == 0); // at least one value must be non-zero
    }

    /// <summary>Produces a value in the range [0, ulong.MaxValue].</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] // small-ish hot path used by a handful of "next" methods
    internal ulong NextUInt64()
    {
        ulong s0 = _s0, s1 = _s1, s2 = _s2, s3 = _s3;

        ulong result = BitOperations.RotateLeft(s1 * 5, 7) * 9;
        ulong t = s1 << 17;

        s2 ^= s0;
        s3 ^= s1;
        s1 ^= s2;
        s0 ^= s3;

        s2 ^= t;
        s3 = BitOperations.RotateLeft(s3, 45);

        _s0 = s0;
        _s1 = s1;
        _s2 = s2;
        _s3 = s3;

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float NextSingle() => (NextUInt64() >> 40) * (1.0f / (1u << 24));
}