using MvvmCross.ViewModels;

namespace PaulN10V.MvxTabbedNavigation.Platforms.Android.ViewModels;

public interface INativeViewModelHolder
{
    public IMvxViewModel? ViewModel { get; }
}