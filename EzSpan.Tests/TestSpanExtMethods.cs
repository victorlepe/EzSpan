using EzSpan.Tests.CustomComparers;

namespace EzSpan.Tests;


public class TestSpanExtMethods
{
    private static ReadOnlySpan<T> AsReadOnly<T>(Span<T> span) => span;
    
    [Fact]
    public void Aggregate()
    {
        ReadOnlySpan<int> span = new[] { 0, 1, 2, 3, 4 };
        
        var accumulator = (int v1, int v2) => v1 + v2;
        var resultSelector = (int v) => v.ToString();
        
        Assert.Equal(10, span.Aggregate(accumulator));
        Assert.Equal(110, span.Aggregate(100, accumulator));
        Assert.Equal(110.ToString(), span.Aggregate(100, accumulator, resultSelector));
        // Can't use Assert.Throws (span is ref struct)
        AssertSpan.Throws<int, InvalidOperationException>(ReadOnlySpan<int>.Empty, s => s.Aggregate(accumulator));
    }

    [Fact]
    public void All()
    {
        ReadOnlySpan<int> span = new[] { 0, 1, 2, 3, 4 };
        
        Assert.True(span.All(v => v < 100));
        Assert.False(span.All(v => v > 100));
        Assert.True(ReadOnlySpan<int>.Empty.All(v => v > 100));
    }

    [Fact]
    public void Any()
    {
        ReadOnlySpan<int> span = new[] { 0, 1, 2, 3, 4 };
        
        Assert.True(span.Any());
        Assert.False(ReadOnlySpan<int>.Empty.Any());
        
        Assert.True(span.Any(v => v > 3));
        Assert.True(span.Any(v => v > 0));
        Assert.False(span.Any(v => v > 100));
    }

    [Fact]
    public void Average()
    {
        // Not nullable
        {
            var array = new[] { 0, 1, 2, 3, 4 };
            ReadOnlySpan<int> span = array;
            var arrayOverflow = new[] { long.MaxValue, long.MaxValue, long.MaxValue };
            
            // No overflow
            Assert.Equal(array.Average(), span.Average());
            
            // Overflow
            
            Assert.Throws<OverflowException>(() => arrayOverflow.Average());
            AssertSpan.Throws<long, OverflowException>(arrayOverflow, s => s.Average());
        }
        
        // Nullable
        {
            var array1 = new[] { 0, 1, 2, 3, 4 };
            var array2 = new int?[] { 0, null, 1, null, 2, null, 3, null, 4, null };
            ReadOnlySpan<int> span1 = array1;
            ReadOnlySpan<int?> span2 = array2;
            var arrayOverflow1 = new[] { long.MaxValue, long.MaxValue, long.MaxValue };
            var arrayOverflow2 = new long?[] { long.MaxValue, null, long.MaxValue, null, long.MaxValue };

            double result1 = span1.Average();
            double? result2 = span2.Average();
            
            Assert.NotNull(result2);
            Assert.Equal(result1, result2.Value);
            
            Assert.Equal(array2.Average(), result2.Value);
            
            
            Assert.Throws<OverflowException>(() => arrayOverflow2.Average());
            AssertSpan.Throws<long, OverflowException>(arrayOverflow1, s => s.Average());
            AssertSpan.Throws<long?, OverflowException>(arrayOverflow2, s => s.Average());
        } 
    }

    [Fact]
    public void Contains()
    {
        var array = new[] { 0, 1, 2, 3, 4 };
        var span = (ReadOnlySpan<int>)array;
        var allwaysFalse = new CustomEqualityComparer<int>((_, _) => false);
        var allwaysTrue = new CustomEqualityComparer<int>((_, _) => true);
        
        Assert.Equal(array.Contains(4), span.Contains(4));
        Assert.Equal(array.Contains(4, allwaysFalse), span.Contains(4, allwaysFalse));
        Assert.Equal(array.Contains(5), span.Contains(5));
        Assert.Equal(array.Contains(5, allwaysTrue), span.Contains(5, allwaysTrue));
    }

    [Fact]
    public void CountAndLongCount()
    {
        // ReSharper disable UseCollectionCountProperty
#pragma warning disable CA1829
#pragma warning disable CS0618
        var array = new[] { 0, 1, 2, 3, 4 };
        var span = (ReadOnlySpan<int>)array;
        
        int arrayCount = array.Count();

        int spanCount = span.Count();

        long spanCountLong = span.LongCount();

        int arrayCount2 = array.Count(v => v > 2);
        int spanCount2 = span.Count(v => v > 2);
        long spanCount2Long = span.LongCount(v => v > 2);
        
        Assert.Equal(arrayCount, spanCount);
        Assert.Equal(span.Length, spanCount);
        Assert.Equal(arrayCount2, spanCount2);
        
        Assert.Equal(spanCount, spanCountLong);
        Assert.Equal(spanCount2, spanCount2Long);
#pragma warning restore CA1829
#pragma warning restore CS0618
        // ReSharper restore UseCollectionCountProperty
    }

    [Fact]
    public void ElementAt()
    {
#pragma warning disable CS0618
        var array = new[] { 0, 1, 2, 3, 4 };
        var span = (ReadOnlySpan<int>)array;
        
        Assert.Equal(span[1], span.ElementAt(1));

        Assert.Equal(span[^1], span.ElementAt(^1));
        AssertSpan.ThrowsAny(span, s => s.ElementAt(10));
        AssertSpan.ThrowsAny(span, s => s.ElementAt(-10));
        Assert.Equal(array.ElementAtOrDefault(-10), span.ElementAtOrDefault(-10));
#pragma warning restore CS0618
    }

    [Fact]
    public void First()
    {
        var array = new[] { 0, 1, 2, 3, 4 };
        var span = (ReadOnlySpan<int>)array;
        
        Assert.Equal(array.First(), span.First());
        Assert.Equal(array.First(v => v > 0), span.First(v => v > 0));
        Assert.Throws<InvalidOperationException>(() => array.First(v => v > 100));
        AssertSpan.Throws<int, InvalidOperationException>(span, s => s.First(v => v > 100));
        
        
        Assert.Equal(array.FirstOrDefault(), span.FirstOrDefault());
        Assert.Equal(array.FirstOrDefault(v => v > 0), span.FirstOrDefault(v => v > 0));
        Assert.Equal(array.FirstOrDefault(v => v > 100), span.FirstOrDefault(v => v > 100));
        
        Assert.Equal(array.FirstOrDefault(-100), span.FirstOrDefault(-100));
        Assert.Equal(array.FirstOrDefault(v => v > 0, -100), span.FirstOrDefault(v => v > 0, -100));
        Assert.Equal(array.FirstOrDefault(v => v > 100, -100), span.FirstOrDefault(v => v > 100, -100));
    }

    [Fact]
    public void Last()
    {
        var array = new[] { 0, 1, 2, 3, 4 };
        var span = (ReadOnlySpan<int>)array;
        
        Assert.Equal(array.Last(), span.Last());
        Assert.Equal(array.Last(v => v > 0), span.Last(v => v > 0));
        Assert.Throws<InvalidOperationException>(() => array.Last(v => v > 100));
        AssertSpan.Throws<int, InvalidOperationException>(span, s => s.Last(v => v > 100));
        
        
        Assert.Equal(array.LastOrDefault(), span.LastOrDefault());
        Assert.Equal(array.LastOrDefault(v => v > 0), span.LastOrDefault(v => v > 0));
        Assert.Equal(array.LastOrDefault(v => v > 100), span.LastOrDefault(v => v > 100));
        
        Assert.Equal(array.LastOrDefault(-100), span.LastOrDefault(-100));
        Assert.Equal(array.LastOrDefault(v => v > 0, -100), span.LastOrDefault(v => v > 0, -100));
        Assert.Equal(array.LastOrDefault(v => v > 100, -100), span.LastOrDefault(v => v > 100, -100));
    }

    [Fact]
    public void Max()
    {
        var array1 = new[] { 0, 1, 2, 3, 4 };
        var array2 = new[] { 0, 4, 1, 2, 3 };
        var array3 = new[] { new { Value = 0 }, new { Value = 4 }, new { Value = 1 }, new { Value = 2 }, new { Value = 3 } };
        
        ReadOnlySpan<int> span1 = array1;
        ReadOnlySpan<int> span2 = array2;
        var span3 = AsReadOnly(array3.AsSpan());

        var reverseComparer = new CustomComparer<int>((v1, v2) => -v1.CompareTo(v2));
        
        Assert.Equal(array1.Max(), span1.Max());
        Assert.Equal(array2.Max(), span2.Max());
        
        Assert.Equal(array1.Max(reverseComparer), span1.Max(reverseComparer));
        Assert.Equal(array2.Max(reverseComparer), span2.Max(reverseComparer));
        
        Assert.Equal(array3.MaxBy(a => a.Value), span3.MaxBy(a => a.Value));
        Assert.Equal(array3.MaxBy(a => a.Value, reverseComparer), span3.MaxBy(a => a.Value, reverseComparer));
    }
    
    [Fact]
    public void Min()
    {
        var array1 = new[] { 0, 1, 2, 3, 4 };
        var array2 = new[] { 0, 4, 1, 2, 3 };
        var array3 = new[] { new { Value = 0 }, new { Value = 4 }, new { Value = 1 }, new { Value = 2 }, new { Value = 3 } };
        
        ReadOnlySpan<int> span1 = array1;
        ReadOnlySpan<int> span2 = array2;
        var span3 = AsReadOnly(array3.AsSpan());

        var reverseComparer = new CustomComparer<int>((v1, v2) => -v1.CompareTo(v2));
        
        Assert.Equal(array1.Min(), span1.Min());
        Assert.Equal(array2.Min(), span2.Min());
        
        Assert.Equal(array1.Min(reverseComparer), span1.Min(reverseComparer));
        Assert.Equal(array2.Min(reverseComparer), span2.Min(reverseComparer));
        
        Assert.Equal(array3.MinBy(a => a.Value), span3.MinBy(a => a.Value));
        Assert.Equal(array3.MinBy(a => a.Value, reverseComparer), span3.MinBy(a => a.Value, reverseComparer));
    }

    [Fact]
    public void Single()
    {
        var array = new[] { 0, 1, 2, 3, 4 };
        var arraySingle = new[] { 0 };
        int[] arrayEmpty = Array.Empty<int>();
        ReadOnlySpan<int> span = array;
        ReadOnlySpan<int> spanSingle = arraySingle;
        ReadOnlySpan<int> spanEmpty = arrayEmpty;
        
        Assert.Throws<InvalidOperationException>(() => array.Single());
        AssertSpan.Throws<int, InvalidOperationException>(span, s => s.Single());
        
        Assert.Throws<InvalidOperationException>(() => arrayEmpty.Single());
        AssertSpan.Throws<int, InvalidOperationException>(spanEmpty, s => s.Single());
        
        Assert.Equal(arraySingle.Single(), spanSingle.Single());
        
        Assert.Throws<InvalidOperationException>(() => array.Single(v => v > 2));
        AssertSpan.Throws<int, InvalidOperationException>(span, s => s.Single(v => v > 2));
        
        Assert.Equal(array.Single(v => v > 3), span.Single(v => v > 3));
        
        
        
        Assert.Throws<InvalidOperationException>(() => array.SingleOrDefault());
        AssertSpan.Throws<int, InvalidOperationException>(span, s => s.SingleOrDefault());
        
        Assert.Equal(arrayEmpty.SingleOrDefault(), spanEmpty.SingleOrDefault());
        Assert.Equal(arrayEmpty.SingleOrDefault(-100), spanEmpty.SingleOrDefault(-100));
        
        Assert.Equal(arraySingle.SingleOrDefault(), spanSingle.SingleOrDefault());
        
        Assert.Throws<InvalidOperationException>(() => array.SingleOrDefault(v => v > 2));
        AssertSpan.Throws<int, InvalidOperationException>(span, s => s.SingleOrDefault(v => v > 2));
        
        Assert.Equal(arraySingle.SingleOrDefault(v => v > 3), spanSingle.SingleOrDefault(v => v > 3));
    }

    [Fact]
    public void SkipAndSkipLast()
    {
        var array = new[] { 0, 1, 2, 3, 4};
        ReadOnlySpan<int> span = array;

        int[] skipLinq1 = array.Skip(0).ToArray();
        int[] skipLinq2 = array.Skip(3).ToArray();
        int[] skipLinq3 = array.Skip(100).ToArray();

        int[] skipSpan1 = span.Skip(0).ToArray();
        int[] skipSpan2 = span.Skip(3).ToArray();
        int[] skipSpan3 = span.Skip(100).ToArray();
        
        Assert.Equal(skipLinq1, skipSpan1);
        Assert.Equal(skipLinq2, skipSpan2);
        Assert.Equal(skipLinq3, skipSpan3);
        
        int[] skipLastLinq1 = array.SkipLast(0).ToArray();
        int[] skipLastLinq2 = array.SkipLast(3).ToArray();
        int[] skipLastLinq3 = array.SkipLast(100).ToArray();

        int[] skipLastSpan1 = span.SkipLast(0).ToArray();
        int[] skipLastSpan2 = span.SkipLast(3).ToArray();
        int[] skipLastSpan3 = span.SkipLast(100).ToArray();
        
        Assert.Equal(skipLastLinq1, skipLastSpan1);
        Assert.Equal(skipLastLinq2, skipLastSpan2);
        Assert.Equal(skipLastLinq3, skipLastSpan3);
    }

    [Fact]
    public void Sum()
    {
        var array = new[] { 0, 1, 2, 3, 4 };
        ReadOnlySpan<int> span = array;
        
        var arrayOverflow = new[] {long.MaxValue, long.MaxValue, long.MaxValue};
        ReadOnlySpan<long> spanOverflow = arrayOverflow;

        var arrayNull = new int?[] { 0, null, 1, null, 2, null, 3, null, 4 };
        ReadOnlySpan<int?> spanNull = arrayNull;

        var arrayOverflowNull = new long?[] { long.MaxValue, null, long.MaxValue, null, long.MaxValue };
        ReadOnlySpan<long?> spanOverflowNull = arrayOverflowNull;
        
        Assert.Equal(array.Sum(), span.Sum());
        Assert.Equal(array.Sum(v => v + 1), span.Sum(v => v+ 1));

        Assert.Throws<OverflowException>(() => arrayOverflow.Sum());
        AssertSpan.Throws<long, OverflowException>(spanOverflow, s => s.Sum());
        
        Assert.Equal(arrayNull.Sum(), spanNull.Sum());
        Assert.Equal(arrayNull.Sum(v => v + 1), spanNull.Sum(v => v + 1));

        Assert.Throws<OverflowException>(() => arrayOverflowNull.Sum());
        AssertSpan.Throws<long?, OverflowException>(spanOverflowNull, s => s.Sum());
    }
    
    [Fact]
    public void TakeAndTakeLast()
    {
        var array = new[] { 0, 1, 2, 3, 4};
        ReadOnlySpan<int> span = array;

        int[] takeLinq1 = array.Take(0).ToArray();
        int[] takeLinq2 = array.Take(3).ToArray();
        int[] takeLinq3 = array.Take(100).ToArray();

        int[] takeSpan1 = span.Take(0).ToArray();
        int[] takeSpan2 = span.Take(3).ToArray();
        int[] takeSpan3 = span.Take(100).ToArray();
        
        Assert.Equal(takeLinq1, takeSpan1);
        Assert.Equal(takeLinq2, takeSpan2);
        Assert.Equal(takeLinq3, takeSpan3);
        
        int[] takeLastLinq1 = array.TakeLast(0).ToArray();
        int[] takeLastLinq2 = array.TakeLast(3).ToArray();
        int[] takeLastLinq3 = array.TakeLast(100).ToArray();

        int[] takeLastSpan1 = span.TakeLast(0).ToArray();
        int[] takeLastSpan2 = span.TakeLast(3).ToArray();
        int[] takeLastSpan3 = span.TakeLast(100).ToArray();
        
        Assert.Equal(takeLastLinq1, takeLastSpan1);
        Assert.Equal(takeLastLinq2, takeLastSpan2);
        Assert.Equal(takeLastLinq3, takeLastSpan3);
    }

    [Fact]
    public void ToDictionary()
    {
        var array = new(int Id, string Name)[]
        {
            (0, "Zero"), (1, "One"), (2, "Two"), (3, "Three"), (4, "Four")
        };
        var span = AsReadOnly(array.AsSpan());
        var allwaysTrue = new CustomEqualityComparer<int>((_, _) => true, _ => 100);
        
        Assert.Equal(array.ToDictionary(a => a.Id), span.ToDictionary(a => a.Id), DictionaryComparer<int, (int Id, string Name)>.Default);
        Assert.Equal(array.ToDictionary(a => a.Id, a => a.Name), span.ToDictionary(a => a.Id, a => a.Name), DictionaryComparer<int, string>.Default);

        Assert.Throws<ArgumentException>(() => array.ToDictionary(a => a.Item1, a => a.Item2, allwaysTrue));
        AssertSpan.Throws<(int, string), ArgumentException>(span, s => s.ToDictionary(a => a.Item1, a => a.Item2, allwaysTrue));
    }

    [Fact]
    public void ToHashSet()
    {
        var array = new[] { 0, 1, 2, 3, 4 };
        ReadOnlySpan<int> span = array;
        var allwaysTrue = new CustomEqualityComparer<int>((_, _) => true, _ => 100);

        Assert.Equal(array.ToHashSet(), span.ToHashSet(), SetComparer<int>.Default);
        Assert.Equal(array.ToHashSet(allwaysTrue), span.ToHashSet(allwaysTrue), SetComparer<int>.Default);
    }

    [Fact]
    public void ToList()
    {
        var array = new[] { 0, 1, 2, 3, 4 };
        ReadOnlySpan<int> span = array;
        Assert.Equal(array.ToList().AsEnumerable(), span.ToList().AsEnumerable());
    }
}