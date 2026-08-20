using Android.Content.Res;
using AndroidX.Fragment.App;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;
using View = Android.Views.View;
using AndroidResource = PaulN10V.MvxTabbedNavigation.Demo.Resource;
// ReSharper disable AccessToStaticMemberViaDerivedType

// ReSharper disable once CheckNamespace
namespace PaulN10V.MvxTabbedNavigation.Demo.Platforms.Android.Views;

internal static class ToolbarExtensions
{
    public static void SetToolbarBackButton(this Fragment fragment, View view)
    {
        if (fragment.ParentFragmentManager.BackStackEntryCount > 1)
        {
            var toolbar = view.FindViewById<Toolbar>(AndroidResource.Id.toolbar);
            TypedArray a = fragment.Activity?.Theme?.ObtainStyledAttributes(AndroidResource.Style.AppTheme, [AndroidResource.Attribute.homeAsUpIndicator]);
            var attributeResourceId = a?.GetResourceId(0, 0);
            a?.Recycle();

            if(attributeResourceId.HasValue)
                toolbar?.SetNavigationIcon(attributeResourceId.Value);

            toolbar?.SetNavigationOnClickListener(new ToolbarBackPressHandler(fragment.ParentFragmentManager));

        }
    }
}