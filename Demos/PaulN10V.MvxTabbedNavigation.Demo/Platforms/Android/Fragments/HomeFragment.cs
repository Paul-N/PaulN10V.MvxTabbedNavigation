using Android.OS;
using Android.Runtime;
using Android.Views;
using PaulN10V.MvxTabbedNavigation.Demo.Core.ViewModels;
using PaulN10V.MvxTabbedNavigation.Platforms.Android.Presenters.Attributes;
using PaulN10V.MvxTabbedNavigation.Platforms.Android.Views;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Views.Fragments;
using View = Android.Views.View;
using PaulN10V.MvxTabbedNavigation.Demo.Platforms.Android.Activities;
using AndroidResource = PaulN10V.MvxTabbedNavigation.Demo.Resource;
// ReSharper disable AccessToStaticMemberViaDerivedType

// ReSharper disable once CheckNamespace
namespace PaulN10V.MvxTabbedNavigation.Demo.Platforms.Android.Fragments;

[Register("me.n10v.paul.tabbednavigation.fragments.HomeFragment")]
[RootFragmentPresentation(HostActivityType = typeof(MainActivity))]
public class HomeFragment : BottomNavigationFragment<HomeViewModel>
{
    public override int ContainerId => AndroidResource.Id.containerTabs;

    public override int BottomNavigationViewId => AndroidResource.Id.navigation;

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        this.EnsureBindingContextIsSet();
        var view = this.BindingInflate(AndroidResource.Layout.fragment_home, null);

        return view;
    }
}