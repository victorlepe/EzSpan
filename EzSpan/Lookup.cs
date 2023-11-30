using System.Collections;
// ReSharper disable ForCanBeConvertedToForeach

namespace EzSpan;

internal sealed class Lookup<TKey, TElement> : ILookup<TKey, TElement>
{
    private sealed class Group : IGrouping<TKey, TElement>
    {
        public Group? NextInOrder;
        public Group? NextHashGroup;
        public readonly int HashCode;
        private readonly List<TElement> _element;
        
        public TKey Key { get; }

        public Group(TKey? key, int hashCode)
        {
            Key = key!;
            HashCode = hashCode;
            _element = new List<TElement>();
        }

        internal void Add(TElement element)
        {
            _element.Add(element);
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            return _element.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    private readonly IEqualityComparer<TKey> _comparer;
    private Group?[] _groups;
    private Group? _firstGroup;
    private Group? _lastGroup;
    private int _count;

    private Lookup(IEqualityComparer<TKey>? comparer)
    {
        _comparer = comparer ?? EqualityComparer<TKey>.Default;
        _groups = new Group[7];
    }
    
    internal static Lookup<TKey, TElement> Create<T>(ReadOnlySpan<T> span, Func<T, TKey> keySelector, Func<T, TElement> elementSelector, IEqualityComparer<TKey>? comparer)
    {
        var lookup = new Lookup<TKey, TElement>(comparer);
        for (var i = 0; i < span.Length; i++)
        {
            lookup.GetGroup(keySelector(span[i]), true)!.Add(elementSelector(span[i]));
        }

        return lookup;
    }
    
    internal static Lookup<TKey, T> Create<T>(ReadOnlySpan<T> span, Func<T, TKey> keySelector, IEqualityComparer<TKey>? comparer)
    {
        var lookup = new Lookup<TKey, T>(comparer);
        for (var i = 0; i < span.Length; i++)
        {
            lookup.GetGroup(keySelector(span[i]), true)!.Add(span[i]);
        }

        return lookup;
    }
    
    private Group? GetGroup(TKey? key, bool create)
    {
        int hc = key is null ? 0 : _comparer.GetHashCode(key) & 0x7FFFFFFF;
        var group = _groups[hc % _groups.Length];
        while (group is not null)
        {
            if (group.HashCode == hc && _comparer.Equals(group.Key, key))
                return group;
            group = group.NextHashGroup;
        }

        if (create)
        {
            if (_count == _groups.Length)
                Resize();
            var newGroup = new Group(key, hc);
            int index = hc % _groups.Length;
            newGroup.NextHashGroup = _groups[index];
            _groups[index] = newGroup;

            if (_firstGroup is null)
            {
                _firstGroup = _lastGroup = newGroup;
            }
            else
            {
                _lastGroup!.NextInOrder = newGroup;
                _lastGroup = newGroup;
            }

            _count++;
            return newGroup;
        }

        return null;
    }

    private void Resize()
    {
        int newSize = checked(_count * 2 + 1);
        var newGroups = new Group?[newSize];
        var g = _firstGroup!;
        do
        {
            int index = g.HashCode % newSize;
            g.NextHashGroup = newGroups[index];
            newGroups[index] = g;
        } while ((g = g.NextInOrder) != null);

        _groups = newGroups;
    }

    public int Count => _count;

    private IEnumerable<IGrouping<TKey, TElement>> Enumerate()
    {
        var g = _firstGroup;
        while (g is not null)
        {
            yield return g;
            g = g.NextInOrder;
        }
    }

    public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator() => Enumerate().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public bool Contains(TKey key) => GetGroup(key, false) is not null;
    public IEnumerable<TElement> this[TKey? key] => GetGroup(key, false) ?? Array.Empty<TElement>().AsEnumerable();
}