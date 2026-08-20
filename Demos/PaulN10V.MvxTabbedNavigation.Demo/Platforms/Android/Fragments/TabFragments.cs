using Android.Runtime;
using PaulN10V.MvxTabbedNavigation.Demo.Core.Model;
using PaulN10V.MvxTabbedNavigation.Demo.Core.ViewModels;
using PaulN10V.MvxTabbedNavigation.Platforms.Android.Presenters.Attributes;
using PaulN10V.MvxTabbedNavigation.Presenters.Attributes;
using CoreResource = PaulN10V.MvxTabbedNavigation.Demo.Core.Resource;
using AndroidResource = PaulN10V.MvxTabbedNavigation.Demo.Resource;
// ReSharper disable AccessToStaticMemberViaDerivedType

// ReSharper disable once CheckNamespace
namespace PaulN10V.MvxTabbedNavigation.Demo.Platforms.Android.Fragments;

[TabPresentation(IconResourceId = AndroidResource.Drawable.ic_one, TabId = TabNames.TabOne, TabTitle = CoreResource.OneTab)]
[Register("me.n10v.paul.tabbednavigation.fragments.Tab1Fragment")]
internal class Tab1Fragment : BaseFragment<Tab1ViewModel>  { }

[TabPresentation(IconResourceId = AndroidResource.Drawable.ic_two, TabId = TabNames.TabTwo, TabTitle = CoreResource.TwoTab)]
[Register("me.n10v.paul.tabbednavigation.fragments.Tab2Fragment")]
internal class Tab2Fragment : BaseFragment<Tab2ViewModel> { }

[TabPresentation(IconResourceId = AndroidResource.Drawable.ic_three, TabId = TabNames.TabThree, TabTitle = CoreResource.ThreeTab)]
[Register("me.n10v.paul.tabbednavigation.fragments.Tab3Fragment")]
internal class Tab3Fragment : BaseFragment<Tab3ViewModel> { }

[Register("me.n10v.paul.tabbednavigation.fragments.NewScreenFragment")]
internal class NewScreenFragment : BaseFragment<NewScreenViewModel> { }

[OverTopPresentation]
[Register("me.n10v.paul.tabbednavigation.fragments.OverTopFragment")]
internal class OverTopFragment : BaseFragment<OverTopViewModel> { }