using PaulN10V.MvxTabbedNavigation.Demo.Core.Model;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.Presenters.Hints;
using MvvmCross.ViewModels;
using System.Drawing;
using System.Windows.Input;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global

namespace PaulN10V.MvxTabbedNavigation.Demo.Core.ViewModels;

public sealed class SecureDataTabViewModel : MvxViewModel, IHasColor
{
    private MvxSubscriptionToken _subscription;
    private readonly IMvxNavigationService _navigationService;

    private readonly string _pleaseLoginString = "Please login to see secured string";

    private readonly string _securedString = "Secure string, you see it";

    private string _stringValue;

    public string StringValue
    {
        get => _stringValue;
        set => SetProperty(ref _stringValue, value);
    }


    private bool _goAuthIsVisible;

    public bool GoAuthIsVisible
    {
        get => _goAuthIsVisible;
        set => SetProperty(ref _goAuthIsVisible, value);
    }

    public ICommand GoAuthCommand => new MvxCommand(() => _navigationService.ChangePresentation(new MvxPagePresentationHint(typeof(PhoneViewModel))));

    public Color Color { get; private set; }

    public SecureDataTabViewModel(IMvxMessenger messenger, IMvxNavigationService navigationService)
    {
        _navigationService = navigationService;
        StringValue = _pleaseLoginString;
        GoAuthIsVisible = true;
        _subscription = messenger.Subscribe<LoggedInMessage>((_) => 
        { 
            StringValue = _securedString;
            GoAuthIsVisible = false;
        });
        Color = Resource.GetRandomColor();
    }

    public override void ViewDestroy(bool viewFinishing = true)
    {
        _subscription?.Dispose();
        _subscription = null;
        base.ViewDestroy(viewFinishing);
    }
}