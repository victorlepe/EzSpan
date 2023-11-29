using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
// ReSharper disable ForCanBeConvertedToForeach

namespace EzSpan;

public static class SpanEnum
{
    /// <summary>
    /// Starts a deferred enumeration on the provided span.
    /// </summary>
    /// <param name="span">The span to be enumerated.</param>
    /// <typeparam name="T">The underlying data type.</typeparam>
    /// <returns>A new <see cref="SpanEnum{T}"/> that represents a deferred execution.</returns>
    public static SpanEnum<T> StartEnumeration<T>(this Span<T> span) => new(span);
    
    /// <summary>
    /// Starts a deferred enumeration on the provided span.
    /// </summary>
    /// <param name="span">The span to be enumerated.</param>
    /// <typeparam name="T">The underlying data type.</typeparam>
    /// <returns>A new <see cref="SpanEnum{T}"/> that represents a deferred execution.</returns>
    public static SpanEnum<T> StartEnumeration<T>(this ReadOnlySpan<T> span) => new(span);


    public static TResult Aggregate<TSource, TAccumulate, TResult>(
        this ReadOnlySpan<TSource> span, 
        TAccumulate seed, 
        Func<TAccumulate, TSource, TAccumulate> accumulator,
        Func<TAccumulate, TResult> resultSelector)
    {
        return resultSelector(Aggregate(span, seed, accumulator));
    }

    public static TAccumulate Aggregate<TSource, TAccumulate>(
        this ReadOnlySpan<TSource> span,
        TAccumulate seed,
        Func<TAccumulate, TSource, TAccumulate> accumulator)
    {
        foreach (var t in span)
        {
            seed = accumulator(seed, t);
        }

        return seed;
    }

    public static TSource Aggregate<TSource>(
        this ReadOnlySpan<TSource> span,
        Func<TSource, TSource, TSource> accumulator)
    {
        if(span.IsEmpty)
            ThrowUtils.ThrowNoElements();
        return Aggregate(span, span[0], accumulator);
    }
    
    public static bool All<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate)
    {
        foreach (var t in span)
        {
            if (!predicate(t))
                return false;
        }

        return true;
    }
    
    public static bool Any<T>(this ReadOnlySpan<T> span) => !span.IsEmpty;

    public static bool Any<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate)
    {
        foreach (var t in span)
        {
            if (predicate(t))
                return true;
        }

        return false;
    }

    private static TResult Average<T, TResult>(ReadOnlySpan<T> span) 
        where T : struct, INumberBase<T> 
        where TResult : struct, INumberBase<TResult>
    {
        if(span.IsEmpty)
            ThrowUtils.ThrowNoElements();
        var sum = span[0];
        for (int i = 1; i < span.Length; i++)
        {
            checked
            {
                sum += span[i];
            }
        }

        return TResult.CreateChecked(sum) / TResult.CreateChecked(span.Length);
    }
    
    private static TResult? Average<T, TResult>(ReadOnlySpan<T?> span) 
        where T : struct, INumberBase<T> 
        where TResult : struct, INumberBase<TResult>
    {
        if(span.IsEmpty)
            ThrowUtils.ThrowNoElements();

        var i = 0;
        for (; i < span.Length; i++)
        {
            if (span[i].HasValue)
            {
                var sum = span[i++]!.Value;
                var count = 1;
                for (; i < span.Length; i++)
                {
                    if (span[i].HasValue)
                    {
                        checked
                        {
                            sum += span[i]!.Value;
                            count++;
                        }
                    }
                }

                return TResult.CreateChecked(sum) / TResult.CreateChecked(count);
            }
        }

        return null;
    }

    public static double Average(this ReadOnlySpan<byte> span) => Average<byte, double>(span);
    public static double Average(this ReadOnlySpan<sbyte> span) => Average<sbyte, double>(span);
    public static double Average(this ReadOnlySpan<ushort> span) => Average<ushort, double>(span);
    public static double Average(this ReadOnlySpan<short> span) => Average<short, double>(span);
    public static double Average(this ReadOnlySpan<uint> span) => Average<uint, double>(span);
    public static double Average(this ReadOnlySpan<int> span) => Average<int, double>(span);
    public static double Average(this ReadOnlySpan<ulong> span) => Average<ulong, double>(span);
    public static double Average(this ReadOnlySpan<long> span) => Average<long, double>(span);
    public static double Average(this ReadOnlySpan<float> span) => Average<float, double>(span);
    public static double Average(this ReadOnlySpan<double> span) => Average<double, double>(span);
    public static decimal Average(this ReadOnlySpan<decimal> span) => Average<decimal, decimal>(span);
    
    public static double? Average(this ReadOnlySpan<byte?> span) => Average<byte, double>(span);
    public static double? Average(this ReadOnlySpan<sbyte?> span) => Average<sbyte, double>(span);
    public static double? Average(this ReadOnlySpan<ushort?> span) => Average<ushort, double>(span);
    public static double? Average(this ReadOnlySpan<short?> span) => Average<short, double>(span);
    public static double? Average(this ReadOnlySpan<uint?> span) => Average<uint, double>(span);
    public static double? Average(this ReadOnlySpan<int?> span) => Average<int, double>(span);
    public static double? Average(this ReadOnlySpan<ulong?> span) => Average<ulong, double>(span);
    public static double? Average(this ReadOnlySpan<long?> span) => Average<long, double>(span);
    public static double? Average(this ReadOnlySpan<float?> span) => Average<float, double>(span);
    public static double? Average(this ReadOnlySpan<double?> span) => Average<double, double>(span);
    public static decimal? Average(this ReadOnlySpan<decimal?> span) => Average<decimal, decimal>(span);
    
    public static bool Contains<T>(this ReadOnlySpan<T> span, T value) => span.Contains(value, EqualityComparer<T>.Default);
    
    public static bool Contains<T>(this ReadOnlySpan<T> span, T value, IEqualityComparer<T> comparer)
    {
        foreach (var t in span)
        {
            if (comparer.Equals(value, t))
                return true;
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete("Use span.Length")]
    public static int Count<T>(this ReadOnlySpan<T> span) => span.Length;

    public static int Count<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate)
    {
        int count = 0;
        foreach (var value in span)
        {
            if (predicate(value))
                count++;
        }

        return count;
    }

    [Obsolete("The count can't be more than int.maxvalue")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long LongCount<T>(this ReadOnlySpan<T> span) => span.Length;

    [Obsolete("The count can't be more than int.maxvalue")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long LongCount<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate) => span.Count(predicate);

    [Obsolete("Same as indexing the span")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ElementAt<T>(this ReadOnlySpan<T> span, int index) => span[index];
    
    [Obsolete("Same as indexing the span")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ElementAt<T>(this ReadOnlySpan<T> span, Index index) => span[index];

    public static T? ElementAtOrDefault<T>(this ReadOnlySpan<T> span, int index) => index >= 0 && index < span.Length ? span[index] : default;

    public static T? ElementAtOrDefault<T>(this ReadOnlySpan<T> span, Index index)
    {
        if (!index.IsFromEnd)
            return index.Value < span.Length ? span[index.Value] : default;
        // ReSharper disable once UseIndexFromEndExpression
        return span.Length < index.Value ? default : span[span.Length - index.Value];
    }
    
    
    private static bool TryGetFirst<T>(ReadOnlySpan<T> span, Func<T, bool> predicate, [NotNullWhen(true)] out T? result)
    {
        if(span.IsEmpty)
            ThrowUtils.ThrowNoElements();
        for (int i = 0; i < span.Length; i++)
        {
            if (predicate(span[i]))
            {
                result = span[i]!;
                return true;
            }
        }

        result = default;
        return false;
    }
    
    private static bool TryGetSingle<T>(ReadOnlySpan<T> span, Func<T, bool> predicate, [NotNullWhen(true)] out T? result)
    {
        if(span.IsEmpty)
            ThrowUtils.ThrowNoElements();
        for (int i = 0; i < span.Length; i++)
        {
            result = span[i]!;
            if (predicate(result))
            {
                i++;
                for (; i < span.Length; i++)
                {
                    if(predicate(span[i]))
                        ThrowUtils.ThrowMoreThanOneMatch();
                }

                return true;
            }
        }

        result = default;
        return false;
    }

    private static bool TryGetLast<T>(ReadOnlySpan<T> span, Func<T, bool> predicate, [NotNullWhen(true)] out T? result)
    {
        if(span.IsEmpty)
            ThrowUtils.ThrowNoElements();
        
        for (int i = span.Length - 1; i >= 0; i--)
        {
            if (predicate(span[i]))
            {
                result = span[i]!;
                return true;
            }
        }

        result = default;
        return false;
    }
    
    public static T First<T>(this ReadOnlySpan<T> span)
    {
        if(span.IsEmpty)
            ThrowUtils.ThrowNoElements();
        return span[0];
    }
    
    public static T First<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate)
    {
        if (TryGetFirst(span, predicate, out var result))
            return result;
        ThrowUtils.ThrowNoMatch();
        return default!;
    }

    public static T? FirstOrDefault<T>(this ReadOnlySpan<T> span)
    {
        return span.IsEmpty ? default : span[0];
    }
    
    public static T? FirstOrDefault<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate)
    {
        return TryGetFirst(span, predicate, out var result) ? result : default;
    }

    public static T FirstOrDefault<T>(this ReadOnlySpan<T> span, T defaultValue)
    {
        return span.IsEmpty ? defaultValue : span[0];
    }
    
    public static T FirstOrDefault<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate, T defaultValue)
    {
        return TryGetFirst(span, predicate, out var result) ? result : defaultValue;
    }
    
    public static T Last<T>(this ReadOnlySpan<T> span)
    {
        if(span.IsEmpty)
            ThrowUtils.ThrowNoElements();
        return span[^1];
    }

    public static T Last<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate)
    {
        if (TryGetLast(span, predicate, out var result))
            return result;
        ThrowUtils.ThrowNoMatch();
        return default!;
    }

    public static T? LastOrDefault<T>(this ReadOnlySpan<T> span)
    {
        return span.IsEmpty ? default : span[^1];
    }

    public static T? LastOrDefault<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate)
    {
        return TryGetLast(span, predicate, out var result) ? result : default;
    }

    public static T LastOrDefault<T>(this ReadOnlySpan<T> span, T defaultValue)
    {
        return span.IsEmpty ? defaultValue : span[^1];
    }
    
    public static T LastOrDefault<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate, T defaultValue)
    {
        return TryGetLast(span, predicate, out var result) ? result : defaultValue;
    }

    public static T Max<T>(this ReadOnlySpan<T> span) => span.Max(Comparer<T>.Default);

    public static T Max<T>(this ReadOnlySpan<T> span, IComparer<T> comparer)
    {
        if(span.IsEmpty)
            ThrowUtils.ThrowNoElements();
        var max = span[0];
        for (int i = 1; i < span.Length; i++)
        {
            if (comparer.Compare(max, span[i]) < 0)
                max = span[i];
        }
        return max;
    }

    public static T MaxBy<T, TKey>(this ReadOnlySpan<T> span, Func<T, TKey> selector) => span.MaxBy(selector, Comparer<TKey>.Default);
    
    public static T MaxBy<T, TKey>(this ReadOnlySpan<T> span, Func<T, TKey> selector, IComparer<TKey> comparer)
    {
        if(span.IsEmpty)
            ThrowUtils.ThrowNoElements();
        var max = span[0];
        var maxBy = selector(max);
        for (int i = 1; i < span.Length; i++)
        {
            var key = selector(span[i]);
            if (comparer.Compare(maxBy, key) < 0)
            {
                max = span[i];
                maxBy = key;
            }
        }

        return max;
    }

    public static T Min<T>(this ReadOnlySpan<T> span) => span.Min(Comparer<T>.Default);
    public static T Min<T>(this ReadOnlySpan<T> span, IComparer<T> comparer)
    {
        if(span.IsEmpty)
            ThrowUtils.ThrowNoElements();
        var min = span[0];
        for (int i = 1; i < span.Length; i++)
        {
            if (comparer.Compare(min, span[i]) > 0)
                min = span[i];
        }
        return min;
    }
    
    public static T MinBy<T, TKey>(this ReadOnlySpan<T> span, Func<T, TKey> selector) => span.MinBy(selector, Comparer<TKey>.Default);
    
    public static T MinBy<T, TKey>(this ReadOnlySpan<T> span, Func<T, TKey> selector, IComparer<TKey> comparer)
    {
        if(span.IsEmpty)
            ThrowUtils.ThrowNoElements();
        var min = span[0];
        var minBy = selector(min);
        for (int i = 1; i < span.Length; i++)
        {
            var key = selector(span[i]);
            if (comparer.Compare(minBy, key) > 0)
            {
                min = span[i];
                minBy = key;
            }
        }

        return min;
    }
    
    public static T Single<T>(this ReadOnlySpan<T> span)
    {
        if(span.IsEmpty)
            ThrowUtils.ThrowNoElements();
        if(span.Length > 1)
            ThrowUtils.ThrowMoreThanOneElement();
        return span[0];
    }

    public static T Single<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate)
    {
        if (TryGetSingle(span, predicate, out var result))
            return result;
        ThrowUtils.ThrowNoMatch();
        return default!;
    }
    
    public static T? SingleOrDefault<T>(this ReadOnlySpan<T> span)
    {
        if (span.IsEmpty)
            return default;
        if(span.Length > 1)
            ThrowUtils.ThrowMoreThanOneElement();
        return span[0];
    }
    
    public static T? SingleOrDefault<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate)
    {
        return TryGetSingle(span, predicate, out var result) ? result : default;
    }
    
    public static T SingleOrDefault<T>(this ReadOnlySpan<T> span, T defaultValue)
    {
        if (span.IsEmpty)
            return defaultValue;
        if(span.Length > 1)
            ThrowUtils.ThrowMoreThanOneElement();
        return span[0];
    }
    
    public static T SingleOrDefault<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate, T defaultValue)
    {
        return TryGetSingle(span, predicate, out var result) ? result : defaultValue;
    }

    public static ReadOnlySpan<T> Skip<T>(this ReadOnlySpan<T> span, int count)
    {
        if (count < 0)
            ThrowUtils.ThrowOnlyNonNegative(nameof(count));
        return span.Length <= count ? ReadOnlySpan<T>.Empty : span[count..];
    }

    public static ReadOnlySpan<T> SkipLast<T>(this ReadOnlySpan<T> span, int count)
    {
        if (count < 0)
            ThrowUtils.ThrowOnlyNonNegative(nameof(count));
        return span.Length <= count ? ReadOnlySpan<T>.Empty : span[..^count];
    }

    private static T Sum<T>(ReadOnlySpan<T> span) where T : struct, INumberBase<T>
    {
        var sum = T.Zero;
        for (int i = 0; i < span.Length; i++)
        {
            checked
            {
                sum += span[i];
            }
        }

        return sum;
    }
    
    private static TResult Sum<T, TResult>(ReadOnlySpan<T> span, Func<T, TResult> selector) where TResult : struct, INumberBase<TResult>
    {
        var sum = TResult.Zero;
        for (int i = 0; i < span.Length; i++)
        {
            checked
            {
                sum += selector(span[i]);
            }
        }

        return sum;
    }
    
    private static T? Sum<T>(ReadOnlySpan<T?> span) where T : struct, INumberBase<T>
    {
        var i = 0;
        for (; i < span.Length; i++)
        {
            if (span[i].HasValue)
            {
                var sum = span[i++]!.Value;
                for (; i < span.Length; i++)
                {
                    if (span[i].HasValue)
                    {
                        checked
                        {
                            sum += span[i]!.Value;
                        }
                    }
                }

                return sum;
            }
        }

        return null;
    }
    
    private static TResult? Sum<T, TResult>(ReadOnlySpan<T> span, Func<T, TResult?> selector) where TResult : struct, INumberBase<TResult>
    {
        var i = 0;
        for (; i < span.Length; i++)
        {
            var value = selector(span[i]);
            if (value.HasValue)
            {
                var sum = value.Value;
                i++;
                for (; i < span.Length; i++)
                {
                    value = selector(span[i]);
                    if (value.HasValue)
                    {
                        checked
                        {
                            sum += value.Value;
                        }
                    }
                }

                return sum;
            }
        }

        return null;
    }

    public static sbyte Sum(this ReadOnlySpan<sbyte> span) => Sum<sbyte>(span);
    public static ushort Sum(this ReadOnlySpan<ushort> span) => Sum<ushort>(span);
    public static short Sum(this ReadOnlySpan<short> span) => Sum<short>(span);
    public static uint Sum(this ReadOnlySpan<uint> span) => Sum<uint>(span);
    public static int Sum(this ReadOnlySpan<int> span) => Sum<int>(span);
    public static ulong Sum(this ReadOnlySpan<ulong> span) => Sum<ulong>(span);
    public static long Sum(this ReadOnlySpan<long> span) => Sum<long>(span);
    public static float Sum(this ReadOnlySpan<float> span) => Sum<float>(span);
    public static double Sum(this ReadOnlySpan<double> span) => Sum<double>(span);
    public static decimal Sum(this ReadOnlySpan<decimal> span) => Sum<decimal>(span);
    public static byte? Sum(this ReadOnlySpan<byte?> span) => Sum<byte>(span);
    public static sbyte? Sum(this ReadOnlySpan<sbyte?> span) => Sum<sbyte>(span);
    public static ushort? Sum(this ReadOnlySpan<ushort?> span) => Sum<ushort>(span);
    public static short? Sum(this ReadOnlySpan<short?> span) => Sum<short>(span);
    public static uint? Sum(this ReadOnlySpan<uint?> span) => Sum<uint>(span);
    public static int? Sum(this ReadOnlySpan<int?> span) => Sum<int>(span);
    public static ulong? Sum(this ReadOnlySpan<ulong?> span) => Sum<ulong>(span);
    public static long? Sum(this ReadOnlySpan<long?> span) => Sum<long>(span);
    public static float? Sum(this ReadOnlySpan<float?> span) => Sum<float>(span);
    public static double? Sum(this ReadOnlySpan<double?> span) => Sum<double>(span);
    public static decimal? Sum(this ReadOnlySpan<decimal?> span) => Sum<decimal>(span);
    public static byte Sum<T>(this ReadOnlySpan<T> span, Func<T, byte> selector) => Sum<T, byte>(span, selector);
    public static sbyte Sum<T>(this ReadOnlySpan<T> span, Func<T, sbyte> selector) => Sum<T, sbyte>(span, selector);
    public static ushort Sum<T>(this ReadOnlySpan<T> span, Func<T, ushort> selector) => Sum<T, ushort>(span, selector);
    public static short Sum<T>(this ReadOnlySpan<T> span, Func<T, short> selector) => Sum<T, short>(span, selector);
    public static uint Sum<T>(this ReadOnlySpan<T> span, Func<T, uint> selector) => Sum<T, uint>(span, selector);
    public static int Sum<T>(this ReadOnlySpan<T> span, Func<T, int> selector) => Sum<T, int>(span, selector);
    public static ulong Sum<T>(this ReadOnlySpan<T> span, Func<T, ulong> selector) => Sum<T, ulong>(span, selector);
    public static long Sum<T>(this ReadOnlySpan<T> span, Func<T, long> selector) => Sum<T, long>(span, selector);
    public static float Sum<T>(this ReadOnlySpan<T> span, Func<T, float> selector) => Sum<T, float>(span, selector);
    public static double Sum<T>(this ReadOnlySpan<T> span, Func<T, double> selector) => Sum<T, double>(span, selector);
    public static decimal Sum<T>(this ReadOnlySpan<T> span, Func<T, decimal> selector) => Sum<T, decimal>(span, selector);
    public static byte? Sum<T>(this ReadOnlySpan<T> span, Func<T, byte?> selector) => Sum<T, byte>(span, selector);
    public static sbyte? Sum<T>(this ReadOnlySpan<T> span, Func<T, sbyte?> selector) => Sum<T, sbyte>(span, selector);
    public static ushort? Sum<T>(this ReadOnlySpan<T> span, Func<T, ushort?> selector) => Sum<T, ushort>(span, selector);
    public static short? Sum<T>(this ReadOnlySpan<T> span, Func<T, short?> selector) => Sum<T, short>(span, selector);
    public static uint? Sum<T>(this ReadOnlySpan<T> span, Func<T, uint?> selector) => Sum<T, uint>(span, selector);
    public static int? Sum<T>(this ReadOnlySpan<T> span, Func<T, int?> selector) => Sum<T, int>(span, selector);
    public static ulong? Sum<T>(this ReadOnlySpan<T> span, Func<T, ulong?> selector) => Sum<T, ulong>(span, selector);
    public static long? Sum<T>(this ReadOnlySpan<T> span, Func<T, long?> selector) => Sum<T, long>(span, selector);
    public static float? Sum<T>(this ReadOnlySpan<T> span, Func<T, float?> selector) => Sum<T, float>(span, selector);
    public static double? Sum<T>(this ReadOnlySpan<T> span, Func<T, double?> selector) => Sum<T, double>(span, selector);
    public static decimal? Sum<T>(this ReadOnlySpan<T> span, Func<T, decimal?> selector) => Sum<T, decimal>(span, selector);
    
    public static ReadOnlySpan<T> Take<T>(this ReadOnlySpan<T> span, int count)
    {
        if (count < 0)
            ThrowUtils.ThrowOnlyNonNegative(nameof(count));
        return span.Length < count ? span : span[..count];
    }

    public static ReadOnlySpan<T> TakeLast<T>(this ReadOnlySpan<T> span, int count)
    {
        if (count < 0)
            ThrowUtils.ThrowOnlyNonNegative(nameof(count));
        return span.Length <= count ? span : span[^count..];
    }

    public static Dictionary<TKey, T> ToDictionary<T, TKey>(this ReadOnlySpan<T> span,
        Func<T, TKey> keySelector)
        where TKey : notnull
    {
        return span.ToDictionary(keySelector, EqualityComparer<TKey>.Default);
    }
    
    public static Dictionary<TKey, T> ToDictionary<T, TKey>(this ReadOnlySpan<T> span,
        Func<T, TKey> keySelector,
        IEqualityComparer<TKey> keyComparer)
        where TKey : notnull
    {
        var dict = new Dictionary<TKey, T>(keyComparer);
        for (var i = 0; i < span.Length; i++)
            dict.Add(keySelector(span[i]), span[i]);
        return dict;
    }
    
    public static Dictionary<TKey, TValue> ToDictionary<T, TKey, TValue>(this ReadOnlySpan<T> span,
        Func<T, TKey> keySelector,
        Func<T, TValue> valueSelector)
        where TKey : notnull
    {
        return span.ToDictionary(keySelector, valueSelector, EqualityComparer<TKey>.Default);
    }
    
    public static Dictionary<TKey, TValue> ToDictionary<T, TKey, TValue>(this ReadOnlySpan<T> span,
        Func<T, TKey> keySelector,
        Func<T, TValue> valueSelector,
        IEqualityComparer<TKey> keyComparer)
        where TKey : notnull
    {
        var dict = new Dictionary<TKey, TValue>(keyComparer);
        for (var i = 0; i < span.Length; i++)
            dict.Add(keySelector(span[i]), valueSelector(span[i]));
        return dict;
    }

    public static HashSet<T> ToHashSet<T>(this ReadOnlySpan<T> span) => span.ToHashSet(EqualityComparer<T>.Default);
    
    public static HashSet<T> ToHashSet<T>(this ReadOnlySpan<T> span, IEqualityComparer<T> comparer)
    {
        var set = new HashSet<T>(comparer);
        for (int i = 0; i < span.Length; i++)
            set.Add(span[i]);
        return set;
    }

    public static List<T> ToList<T>(this ReadOnlySpan<T> span)
    {
        var list = new List<T>(span.Length);
        for(int i = 0; i < span.Length; i++)
            list.Add(span[i]);
        return list;
    }
    // TODO: ToLookup()
}