using Android.App;
using Android.Runtime;
using PaulN10V.MvxTabbedNavigation.Demo.Core;
using MvvmCross.Platforms.Android.Views;

// ReSharper disable once CheckNamespace
namespace PaulN10V.MvxTabbedNavigation.Demo.Platforms.Android;

[Application]
public class Application : MvxAndroidApplication<Setup, App>
{
    public Application() { }

    public Application(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }
}