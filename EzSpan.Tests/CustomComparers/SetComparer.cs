namespace EzSpan.Tests.CustomComparers;

public sealed class SetComparer<T> : IEqualityComparer<HashSet<T>>
{
    private SetComparer()
    {
    }

    public static SetComparer<T> Default { get; } = new();
    
    public bool Equals(HashSet<T>? first, HashSet<T>? second)
    {
        if (ReferenceEquals(first, second))
            return true;
        if (first is null || second is null)
            return false;
        return first.Count == second.Count && first.All(second.Contains);
    }

    public int GetHashCode(HashSet<T> obj)
    {
        return obj.GetHashCode();
    }
}