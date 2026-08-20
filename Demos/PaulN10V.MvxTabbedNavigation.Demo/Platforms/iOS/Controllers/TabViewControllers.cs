using PaulN10V.MvxTabbedNavigation.Demo.Core;
using PaulN10V.MvxTabbedNavigation.Demo.Core.ViewModels;
using PaulN10V.MvxTabbedNavigation.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Presenters.Attributes;

// ReSharper disable once CheckNamespace
namespace PaulN10V.MvxTabbedNavigation.Demo.Platforms.iOS.Controllers;

[MvxTabPresentation(WrapInNavigationController = true, TabName = Resource.OneTab, TabIconName = "ic_one")]
internal sealed class Tab1ViewController : BaseViewController<Tab1ViewModel> { }

[MvxTabPresentation(WrapInNavigationController = true, TabName = Resource.TwoTab, TabIconName = "ic_two")]
internal sealed class Tab2ViewController : BaseViewController<Tab2ViewModel> { }

[MvxTabPresentation(WrapInNavigationController = true, TabName = Resource.ThreeTab, TabIconName = "ic_three")]
internal sealed class Tab3ViewController : BaseViewController<Tab3ViewModel> { }

internal sealed class NewScreenViewController : BaseViewController<NewScreenViewModel> { }

[OverTopPresentation]
internal sealed class OverTopViewController : BaseViewController<OverTopViewModel> { }