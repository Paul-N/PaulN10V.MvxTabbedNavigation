using MvvmCross.Navigation;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable ConvertToPrimaryConstructor

namespace PaulN10V.MvxTabbedNavigation.Demo.Core.ViewModels;

public sealed class NewScreenViewModel : BaseViewModel
{
    public NewScreenViewModel(IMvxNavigationService navigationService) : base(navigationService) { }
}

public sealed class OverTopViewModel : BaseViewModel
{
    public OverTopViewModel(IMvxNavigationService navigationService) : base(navigationService) { }
}

public sealed class Tab1ViewModel : BaseViewModel
{
    public Tab1ViewModel(IMvxNavigationService navigationService) : base(navigationService) { }
}

public sealed class Tab2ViewModel : BaseViewModel
{
    public Tab2ViewModel(IMvxNavigationService navigationService) : base(navigationService) { }
}

public sealed class Tab3ViewModel : BaseViewModel
{
    public Tab3ViewModel(IMvxNavigationService navigationService) : base(navigationService) { }
}