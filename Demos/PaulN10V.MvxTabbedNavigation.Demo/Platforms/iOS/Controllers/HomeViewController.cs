using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using UIKit;

// ReSharper disable once CheckNamespace
namespace PaulN10V.MvxTabbedNavigation.Demo.Platforms.iOS.Controllers;

[MvxRootPresentation(WrapInNavigationController = true)]
public class HomeViewController : MvxTabBarViewController
{
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();
        TabBar.BackgroundColor = UIColor.White;
    }
}