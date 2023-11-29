namespace EzSpan.Tests.CustomComparers;

public sealed class DictionaryComparer<TKey, TValue> : IEqualityComparer<Dictionary<TKey, TValue>> where TKey : notnull
{
    public static DictionaryComparer<TKey, TValue> Default { get; } = new();
        
    private readonly IEqualityComparer<TValue> _valueComparer;
        
    public DictionaryComparer() : this(EqualityComparer<TValue>.Default)
    {
    }

    public DictionaryComparer(IEqualityComparer<TValue> valueComparer)
    {
        _valueComparer = valueComparer;
    }
        
    public bool Equals(Dictionary<TKey, TValue>? first, Dictionary<TKey, TValue>? second)
    {
        if (ReferenceEquals(first, second))
            return true;

        if (first is null || second is null)
            return false;
        
        if (first.Count != second.Count)
            return false;
        
        foreach (var (key, valueSecond) in second)
        {
            if (!first.TryGetValue(key, out var valueFirst))
                return false;
            if (!_valueComparer.Equals(valueSecond, valueFirst))
                return false;
        }

        return true;
    }

    public int GetHashCode(Dictionary<TKey, TValue> obj)
    {
        return obj.GetHashCode();
    }
}