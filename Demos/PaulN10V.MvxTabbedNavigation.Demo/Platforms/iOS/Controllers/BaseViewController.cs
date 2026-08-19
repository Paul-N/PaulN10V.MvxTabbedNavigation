using PaulN10V.MvxTabbedNavigation.Demo.Core;
using PaulN10V.MvxTabbedNavigation.Demo.Core.ViewModels;
using MvvmCross.Platforms.Ios.Views;
using UIKit;

// ReSharper disable once CheckNamespace
namespace PaulN10V.MvxTabbedNavigation.Demo.Platforms.iOS.Controllers;

internal class BaseViewController<TViewModel> : MvxViewController<TViewModel> where TViewModel : BaseViewModel
{
    private UIButton _btnPlus;
    private UILabel _valLbl;
    private UIButton _btnMinus;
    private UIButton _btnNew;
    private UIButton _btnOverTop;
    private UIButton _btnPopToRoot;
    private UIButton _btnCloseSelf;

    public override void LoadView()
    {
        base.LoadView();

        _btnPlus = new UIButton() { TranslatesAutoresizingMaskIntoConstraints = false, TintColor = UIColor.White };
        _btnPlus.SetImage(UIImage.GetSystemImage("plus.circle.fill"), UIControlState.Normal);
        _btnPlus.WidthAnchor.ConstraintEqualTo(Resource._44px).Active = true;
        _btnPlus.HeightAnchor.ConstraintEqualTo(Resource._44px).Active = true;

        _valLbl = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false, TextColor = UIColor.White, TextAlignment = UITextAlignment.Center, Text = "0" };
        _valLbl.WidthAnchor.ConstraintEqualTo(Resource._44px).Active = true;
        _valLbl.HeightAnchor.ConstraintEqualTo(Resource._44px).Active = true;

        _btnMinus = new UIButton() { TranslatesAutoresizingMaskIntoConstraints = false, TintColor = UIColor.White };
        _btnMinus.SetImage(UIImage.GetSystemImage("minus.circle.fill"), UIControlState.Normal);
        _btnMinus.WidthAnchor.ConstraintEqualTo(Resource._44px).Active = true;
        _btnMinus.HeightAnchor.ConstraintEqualTo(Resource._44px).Active = true;

        UIStackView stateStack = ViewEx.CreateStackView(UILayoutConstraintAxis.Horizontal, _btnMinus, _valLbl, _btnPlus);

        _btnNew = ViewEx.CreateTitledButton(Resource.OpenNew);

        _btnOverTop = ViewEx.CreateTitledButton(Resource.OpenOverTop);
        _btnPopToRoot = ViewEx.CreateTitledButton(Resource.PopToRoot);
        _btnCloseSelf = ViewEx.CreateTitledButton(Resource.Close);

        var stack = ViewEx.CreateStackView(UILayoutConstraintAxis.Vertical, stateStack, _btnNew, _btnOverTop, _btnPopToRoot, _btnCloseSelf);

        View?.AddSubview(stack);

        stack.CenterOf(View);
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        var set = this.CreateBindingSet();
        set.Bind(_btnMinus).To(vm => vm.DecrCommand);
        set.Bind(_btnPlus).To(vm => vm.IncrCommand);
        set.Bind(_valLbl).To(vm => vm.Value);
        set.Bind(_btnNew).To(vm => vm.OpenNewCommand);
        set.Bind(_btnOverTop).To(vm => vm.OpenOverTopCommand);
        set.Bind(_btnPopToRoot).To(vm => vm.PopToRootCommand);
        set.Bind(_btnCloseSelf).To(vm => vm.CloseSelfCommand);
        set.BindBackground(View);
        set.Apply();
    }
}