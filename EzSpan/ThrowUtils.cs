using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace EzSpan;

internal static class ThrowUtils
{
    [StackTraceHidden, DoesNotReturn]
    public static void ThrowNoElements()
    {
        throw new InvalidOperationException("Sequence contains no elements");
    }
    
    [StackTraceHidden, DoesNotReturn]
    public static void ThrowNoMatch()
    {
        throw new InvalidOperationException("No element in the sequence satifies the predicate");
    }
    
    [StackTraceHidden, DoesNotReturn]
    public static void ThrowMoreThanOneElement()
    {
        throw new InvalidOperationException("Sequence contains more than a single element");
    }
    
    [StackTraceHidden, DoesNotReturn]
    public static void ThrowMoreThanOneMatch()
    {
        throw new InvalidOperationException("More than a single element in the sequence satifies the predicate");
    }

    [StackTraceHidden, DoesNotReturn]
    public static void ThrowOnlyNonNegative(string paramName)
    {
        throw new ArgumentOutOfRangeException(paramName, "Only positive allowed");
    }
}