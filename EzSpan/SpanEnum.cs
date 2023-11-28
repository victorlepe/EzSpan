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
}