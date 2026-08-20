using AndroidX.Fragment.App;
using View = Android.Views.View;

// ReSharper disable once CheckNamespace
namespace PaulN10V.MvxTabbedNavigation.Demo.Platforms.Android.Views;

internal class ToolbarBackPressHandler : Java.Lang.Object, View.IOnClickListener
{
    FragmentManager _fragmentManager;

    // ReSharper disable once ConvertToPrimaryConstructor
    public ToolbarBackPressHandler(FragmentManager fragmentManager) 
        => _fragmentManager = fragmentManager;

    public void OnClick(View v) => _fragmentManager?.PopBackStack();

    protected override void Dispose(bool disposing)
    {
        _fragmentManager = null;
        base.Dispose(disposing);
    }
}