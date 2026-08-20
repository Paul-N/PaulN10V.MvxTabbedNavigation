using PaulN10V.MvxTabbedNavigation.Demo.Core;
using PaulN10V.MvxTabbedNavigation.Demo.Core.ViewModels;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using UIKit;

// ReSharper disable once CheckNamespace
namespace PaulN10V.MvxTabbedNavigation.Demo.Platforms.iOS.Controllers;

[MvxTabPresentation(WrapInNavigationController = true, TabName = Resource.LoginTab, TabIconName = "ic_account")]
internal class PhoneViewControllers : MvxViewController<PhoneViewModel>
{
    private UITextField _editTextPhone;
    private UIButton _btnGetCode;

    public override void LoadView()
    {
        base.LoadView();

        var lbl = ViewEx.CreateTitledLabel(Resource.EnterPhone);

        _editTextPhone = ViewEx.CreateTextField();

        _btnGetCode = ViewEx.CreateTitledButton(Resource.GetCode);

        var stack = ViewEx.CreateStackView(UILayoutConstraintAxis.Vertical, lbl, _editTextPhone, _btnGetCode );

        View?.AddSubview(stack);

        stack.CenterOf(View);
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        var set = this.CreateBindingSet();
        set.Bind(_editTextPhone).To(vm => vm.PhoneNumber);
        set.Bind(_btnGetCode).To(vm => vm.NextCommand);
        set.BindBackground(View);
        set.Apply();
    }
}

internal class SmsCodeViewControllers : MvxViewController<SmsCodeViewModel>
{
    private UITextField _editTextSms;
    private UIButton _btnGetCode;

    public override void LoadView()
    {
        base.LoadView();

        var lbl = ViewEx.CreateTitledLabel(Resource.EnterSms);

        _editTextSms = ViewEx.CreateTextField();

        _btnGetCode = ViewEx.CreateTitledButton(Resource.Send);

        var stack = ViewEx.CreateStackView(UILayoutConstraintAxis.Vertical, lbl, _editTextSms, _btnGetCode);

        View?.AddSubview(stack);

        stack.CenterOf(View);
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        var set = this.CreateBindingSet();
        set.Bind(_editTextSms).To(vm => vm.SmsCode);
        set.Bind(_btnGetCode).To(vm => vm.NextCommand);
        set.BindBackground(View);
        set.Apply();
    }
}

internal class AccountViewControllers : MvxViewController<AccountViewModel>
{
    private UILabel _lblAccount;

    public override void LoadView()
    {
        base.LoadView();

        _lblAccount = ViewEx.CreateTitledLabel(String.Empty);

        View?.AddSubview(_lblAccount);

        _lblAccount.CenterOf(View);
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();
        this.Title = Resource.Account;
        var set = this.CreateBindingSet();
        set.Bind(_lblAccount).To(vm => vm.AccountInfo);
        set.BindBackground(View);
        set.Apply();
    }
}