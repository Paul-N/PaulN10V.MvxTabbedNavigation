using Kotlin.Jvm.Functions;
using JavaObject = Java.Lang.Object;

namespace PaulN10V.MvxTabbedNavigation.Platforms.Android.Utils;

internal class KotlinFunction1<T, TResult> : JavaObject, IFunction1 where T : JavaObject where TResult : JavaObject?
{
    private Func<T, TResult>? _dotNetFunc;

    private bool _disposed;

    public KotlinFunction1(Func<T, TResult> dotNetFunc)
    {
        // ReSharper disable once RedundantCallerArgumentExpressionDefaultValue
        ArgumentNullException.ThrowIfNull(dotNetFunc, nameof(dotNetFunc));
        _dotNetFunc = dotNetFunc;
    }

    public JavaObject? Invoke(JavaObject? parameter)
    {
        if (_disposed)
            throw new InvalidOperationException($"Can not invoke {nameof(Func<T, TResult>)} after {nameof(Dispose)}() call");

        if (parameter is T p)
            return _dotNetFunc?.Invoke(p);
        else
            throw new ArgumentNullException(nameof(parameter));
    }

    protected override void Dispose(bool disposing)
    {
        _dotNetFunc = null;
        _disposed = true;
        base.Dispose(disposing);
    }
}