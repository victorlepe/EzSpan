namespace EzSpan;

/// <summary>
/// This struct represents a deferred <see cref="ReadOnlySpan{T}"/> enumeration
/// </summary>
/// <typeparam name="T">The underlying data type</typeparam>
public readonly ref struct SpanEnum<T>
{
    private readonly ReadOnlySpan<T> _span;

    public SpanEnum(ReadOnlySpan<T> span)
    {
        _span = span;
    }
}