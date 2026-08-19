using PaulN10V.MvxTabbedNavigation.Demo.Core;
using PaulN10V.MvxTabbedNavigation.Demo.Core.ViewModels;
using MvvmCross.Platforms.Ios.Views;
using UIKit;

// ReSharper disable once CheckNamespace
namespace PaulN10V.MvxTabbedNavigation.Demo.Platforms.iOS.Controllers;

internal class StartViewController : MvxViewController<StartViewModel>
{
    private UIButton _btStart;

    public override void LoadView()
    {
        base.LoadView();

        var helloLbl = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false, TextColor = UIColor.White, TextAlignment = UITextAlignment.Center, Lines = 1 };
        helloLbl.WidthAnchor.ConstraintEqualTo(Resource.TitleWidth).Active = true;
        helloLbl.HeightAnchor.ConstraintEqualTo(Resource._44px).Active = true;
        helloLbl.Text = Resource.Thanks;

        _btStart = ViewEx.CreateTitledButton(Resource.Start);

        var stack = ViewEx.CreateStackView(UILayoutConstraintAxis.Vertical, helloLbl, _btStart);

        View?.AddSubview(stack);

        stack.CenterOf(View);
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        var set = this.CreateBindingSet();
        set.Bind(_btStart).To(vm => vm.StartCommand);
        set.BindBackground(View);
        set.Apply();
    }
}