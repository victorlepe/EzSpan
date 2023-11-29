using System.Reflection;
using Xunit.Sdk;

namespace EzSpan.Tests;

public static class AssertSpan
{
    public delegate void ActionSpan<T>(ReadOnlySpan<T> span);
    public delegate object? FuncSpan<T>(ReadOnlySpan<T> span);

    private static Exception? RecordException<T>(ReadOnlySpan<T> span, ActionSpan<T> action)
    {
        try
        {
            action(span);
        }
        catch (Exception ex)
        {
            return ex;
        }

        return null;
    }

    private static Exception? RecordException<T>(ReadOnlySpan<T> span, FuncSpan<T> func)
    {
        Task? task;
        try
        {
            task = func(span) as Task;
        }
        catch (Exception ex)
        {
            return ex;
        }
        if (task != null)
            throw new InvalidOperationException("Async not supported");
        return null;
    }

    private static void ThrowsAssert(Type expectedType, Exception? actual, bool inherit)
    {
        if (actual is null)
            throw new ThrowsException(expectedType);

        bool isValidException = inherit
            ? expectedType.GetTypeInfo().IsAssignableFrom(actual.GetType().GetTypeInfo())
            : expectedType == actual.GetType();

        if (!isValidException)
            throw new ThrowsException(expectedType, actual);
    }
    
    public static void ThrowsAny<T>(ReadOnlySpan<T> span, ActionSpan<T> action)
    {
        ThrowsAssert(typeof(Exception), RecordException(span, action), true);
    }

    public static void ThrowsAny<T, TException>(ReadOnlySpan<T> span, ActionSpan<T> action) where TException : Exception
    {
        ThrowsAssert(typeof(TException), RecordException(span, action), true);
    }
    
    public static void Throws<T, TException>(ReadOnlySpan<T> span, ActionSpan<T> action) where TException : Exception
    {
        ThrowsAssert(typeof(TException), RecordException(span, action), false);
    }
}