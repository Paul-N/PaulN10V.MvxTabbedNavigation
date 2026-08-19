using PaulN10V.MvxTabbedNavigation.Demo.Core;
using PaulN10V.MvxTabbedNavigation.Demo.Core.ViewModels;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using UIKit;

// ReSharper disable once CheckNamespace
namespace PaulN10V.MvxTabbedNavigation.Demo.Platforms.iOS.Controllers;

[MvxTabPresentation(WrapInNavigationController = true, TabName = Resource.SecureTab, TabIconName = "ic_lock")]
internal sealed class SecureDataTabViewController : MvxViewController<SecureDataTabViewModel>
{
    private UILabel _valLbl;
    private UIButton _btnGoAuth;

    public override void LoadView()
    {
        base.LoadView();

        _valLbl = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false, TextColor = UIColor.White, TextAlignment = UITextAlignment.Center, Lines = 2 };
        _valLbl.WidthAnchor.ConstraintEqualTo(Resource.TitleWidth).Active = true;
        _valLbl.HeightAnchor.ConstraintEqualTo(Resource._44px).Active = true;

        _btnGoAuth = new UIButton() { TranslatesAutoresizingMaskIntoConstraints = false, TintColor = UIColor.White };
        _btnGoAuth.SetTitle(Resource.GoLogin, UIControlState.Normal);

        _btnGoAuth.WidthAnchor.ConstraintEqualTo(Resource.TitleWidth).Active = true;
        _btnGoAuth.HeightAnchor.ConstraintEqualTo(Resource._44px).Active = true;

        var stack = new UIStackView([_valLbl, _btnGoAuth]);
        stack.Setup(UILayoutConstraintAxis.Vertical);

        View?.AddSubview(stack);

        stack.CenterOf(View);
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        var set = this.CreateBindingSet();
        set.Bind(_valLbl).To(vm => vm.StringValue);
        set.Bind(_btnGoAuth).To(vm => vm.GoAuthCommand);
        set.Bind(_btnGoAuth).For(v => v.BindVisible()).To(vm => vm.GoAuthIsVisible);
        set.BindBackground(View);
        set.Apply();
    }
}