using MvvmCross.Navigation;
using MvvmCross.ViewModels;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable ConvertToPrimaryConstructor

namespace PaulN10V.MvxTabbedNavigation.Demo.Core.ViewModels;

public sealed class HomeViewModel : MvxViewModel
{
    private readonly IMvxNavigationService _navigationService;
    private bool _areTabsShown;

    public HomeViewModel(IMvxNavigationService navigationService) 
        => this._navigationService = navigationService;

    public override void ViewAppeared()
    {
        if (!_areTabsShown)
        {
            _navigationService.Navigate<Tab1ViewModel>();
            _navigationService.Navigate<Tab2ViewModel>();
            _navigationService.Navigate<Tab3ViewModel>();
            _navigationService.Navigate<SecureDataTabViewModel>();
            _navigationService.Navigate<PhoneViewModel>();
            _areTabsShown = true;
        }
    }
}