using Android.Runtime;
using PaulN10V.MvxTabbedNavigation.Demo.Core.Model;
using PaulN10V.MvxTabbedNavigation.Demo.Core.ViewModels;
using PaulN10V.MvxTabbedNavigation.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views.Fragments;
using CoreResource = PaulN10V.MvxTabbedNavigation.Demo.Core.Resource;
using PaulN10V.MvxTabbedNavigation.Platforms.Android.Views;
using Android.OS;
using Android.Views;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using View = Android.Views.View;
using PaulN10V.MvxTabbedNavigation.Demo.Platforms.Android.Views;
using AndroidResource = PaulN10V.MvxTabbedNavigation.Demo.Resource;
// ReSharper disable AccessToStaticMemberViaDerivedType

// ReSharper disable once CheckNamespace
namespace PaulN10V.MvxTabbedNavigation.Demo.Platforms.Android.Fragments;

[TabPresentation(IconResourceId = AndroidResource.Drawable.ic_lock, TabId = TabNames.TabSecure, TabTitle = CoreResource.SecureTab)]
[Register("me.n10v.paul.tabbednavigation.fragments.SecureDataTabFragment")]
internal sealed class SecureDataTabFragment : Fragment<SecureDataTabViewModel>
{
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        this.EnsureBindingContextIsSet();

        var view = this.BindingInflate(AndroidResource.Layout.fragment_secure_data, null);

        view.SetSizeOf(AndroidResource.Id.lblTitle, Resources, CoreResource.TitleWidth, CoreResource._44px);

        view.SetupTitledTextView(AndroidResource.Id.btnGoAuth, Resources, CoreResource.GoLogin);

        this.SetToolbarBackButton(view);

        return view;
    }
}