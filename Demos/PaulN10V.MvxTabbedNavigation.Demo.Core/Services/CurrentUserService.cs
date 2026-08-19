using System.Diagnostics.CodeAnalysis;
using Microsoft.Maui.Storage;

namespace PaulN10V.MvxTabbedNavigation.Demo.Core.Services;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
internal class CurrentUserService : ICurrentUserService
{
    private readonly string _isLoggedInKey = "__isLoggedIn";
    private readonly string _isOnboardingPassedKey = "__isOnboardingPassed";


    public bool IsLoggedIn 
    {
        get => Preferences.Get(_isLoggedInKey, false);
        set => Preferences.Set(_isLoggedInKey, value);
    }
        
    public bool IsOnboardingPassed 
    {
        get => Preferences.Get(_isOnboardingPassedKey, false);
        set => Preferences.Set(_isOnboardingPassedKey, value);
    }
}