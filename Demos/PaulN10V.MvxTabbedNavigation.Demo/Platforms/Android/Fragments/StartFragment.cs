using Android.OS;
using Android.Views;
using PaulN10V.MvxTabbedNavigation.Platforms.Android.Views;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Views.Fragments;
using PaulN10V.MvxTabbedNavigation.Demo.Core.ViewModels;
using CoreResource = PaulN10V.MvxTabbedNavigation.Demo.Core.Resource;
using View = Android.Views.View;
using Android.Runtime;
using PaulN10V.MvxTabbedNavigation.Demo.Platforms.Android.Activities;
using PaulN10V.MvxTabbedNavigation.Platforms.Android.Presenters.Attributes;
using AndroidResource = PaulN10V.MvxTabbedNavigation.Demo.Resource;
// ReSharper disable AccessToStaticMemberViaDerivedType

// ReSharper disable once CheckNamespace
namespace PaulN10V.MvxTabbedNavigation.Demo.Platforms.Android.Fragments;

[Register("me.n10v.paul.tabbednavigation.fragments.StartFragment")]
[RootFragmentPresentation(HostActivityType = typeof(MainActivity))]
internal sealed class StartFragment : Fragment<StartViewModel>
{
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        this.EnsureBindingContextIsSet();

        var view = this.BindingInflate(AndroidResource.Layout.fragment_start, null);

        view.SetupTitledTextView(AndroidResource.Id.txtHello, this.Resources, CoreResource.Thanks);

        view.SetTextTo(AndroidResource.Id.btnStart, CoreResource.Start);

        return view;
    }
}