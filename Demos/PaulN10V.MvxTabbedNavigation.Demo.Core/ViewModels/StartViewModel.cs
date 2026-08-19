using PaulN10V.MvxTabbedNavigation.Demo.Core.Services;
using PaulN10V.MvxTabbedNavigation.Presenters.Hints;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System.Drawing;
using System.Windows.Input;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable ConvertToPrimaryConstructor

namespace PaulN10V.MvxTabbedNavigation.Demo.Core.ViewModels;

public class StartViewModel : MvxViewModel, IHasColor
{
    private readonly IMvxNavigationService _navigationService;
    private readonly ICurrentUserService _currentUserService;

    public Color Color { get; private set; }

    public ICommand StartCommand => new MvxCommand(() =>
    {
        _currentUserService.IsOnboardingPassed = true;
        _navigationService.ChangePresentation(new ClearStackPresentationHint());
        _navigationService.Navigate<HomeViewModel>();
    });

    public StartViewModel(IMvxNavigationService navigationService, ICurrentUserService currentUserService)
    {
        _navigationService = navigationService;
        _currentUserService = currentUserService;
        Color = Resource.GetRandomColor();
    }
}