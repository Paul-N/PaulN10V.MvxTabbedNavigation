using PaulN10V.MvxTabbedNavigation.Demo.Core.Services;
using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.ViewModels;

namespace PaulN10V.MvxTabbedNavigation.Demo.Core;

public class App : MvxApplication
{
    public override void Initialize()
    {
        Mvx.IoCProvider.ConstructAndRegisterSingleton<ICurrentUserService, CurrentUserService>();
        RegisterCustomAppStart<AppStart>();
    }
}