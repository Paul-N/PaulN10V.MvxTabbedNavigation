using PaulN10V.MvxTabbedNavigation.Platforms.Android.Utils;
using Kotlin.Jvm.Functions;
using JavaObject = Java.Lang.Object;

// ReSharper disable once CheckNamespace
namespace System;

public static class FunctionExtensions
{
    public static IFunction1 Kotlinize<T, TResult>(this Func<T, TResult> dotNetFunc) where T : JavaObject where TResult : JavaObject? 
        => new KotlinFunction1<T, TResult>(dotNetFunc);
}