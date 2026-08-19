namespace PaulN10V.MvxTabbedNavigation.Demo.Core.Services;

public interface ICurrentUserService
{
    bool IsLoggedIn { get; set; }

    bool IsOnboardingPassed { get; set; }
}