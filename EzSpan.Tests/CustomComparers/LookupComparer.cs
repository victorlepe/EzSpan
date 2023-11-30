namespace EzSpan.Tests.CustomComparers;

public sealed class LookupComparer<TKey, TElement> : IEqualityComparer<ILookup<TKey, TElement>>
{
    private sealed class GroupComparer : IEqualityComparer<IGrouping<TKey, TElement>>
    {
        private readonly IEqualityComparer<TElement> _elementComparer;

        public GroupComparer(IEqualityComparer<TElement> elementComparer) => _elementComparer = elementComparer;
        
        public bool Equals(IGrouping<TKey, TElement>? first, IGrouping<TKey, TElement>? second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (first is null || second is null)
                return false;
            var firstArray = first.OrderBy(v => v).ToArray();
            var secondArray = second.OrderBy(v => v).ToArray();
            return firstArray.SequenceEqual(secondArray, _elementComparer);
        }

        public int GetHashCode(IGrouping<TKey, TElement>? obj) => obj is null ? 0 : obj.GetHashCode();
    }

    public static LookupComparer<TKey, TElement> Default { get; } = new(EqualityComparer<TElement>.Default);
    
    private readonly GroupComparer _groupComparer;

    public LookupComparer(IEqualityComparer<TElement> elementComparer) => _groupComparer = new GroupComparer(elementComparer);
    
    
    public bool Equals(ILookup<TKey, TElement>? first, ILookup<TKey, TElement>? second)
    {
        if (ReferenceEquals(first, second))
            return true;
        if (first is null || second is null)
            return false;
        if (first.Count != second.Count)
            return false;
        var firstArray = first.OrderBy(g => g.Key).ToArray();
        var secondArray = second.OrderBy(g => g.Key).ToArray();
        return firstArray.SequenceEqual(secondArray, _groupComparer);
    }

    public int GetHashCode(ILookup<TKey, TElement>? obj) => obj is null ? 0 : obj.GetHashCode();
}