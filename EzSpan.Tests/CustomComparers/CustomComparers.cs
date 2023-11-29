namespace EzSpan.Tests.CustomComparers;

internal sealed class CustomEqualityComparer<T> : IEqualityComparer<T>
{
    private readonly Func<T?, T?, bool> _equals;
    private readonly Func<T, int>? _hash;
    
    public CustomEqualityComparer(Func<T?, T?, bool> equals) : this(equals, null)
    {
    }
    
    public CustomEqualityComparer(Func<T?, T?, bool> equals, Func<T, int>? hash)
    {
        _equals = equals;
        _hash = hash;
    }
    
    public bool Equals(T? x, T? y) => _equals(x, y);
    public int GetHashCode(T obj) => _hash is not null ? _hash(obj) : obj?.GetHashCode() ?? 0;
}

internal sealed class CustomComparer<T> : IComparer<T>
{
    private readonly Func<T?, T?, int> _comparer;
    public CustomComparer(Func<T?, T?, int> comparer)
    {
        _comparer = comparer;
    }
    public int Compare(T? x, T? y) => _comparer(x, y);
}