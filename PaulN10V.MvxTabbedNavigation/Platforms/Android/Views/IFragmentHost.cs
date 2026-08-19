using FragmentManager = AndroidX.Fragment.App.FragmentManager;

namespace PaulN10V.MvxTabbedNavigation.Platforms.Android.Views;

public interface IFragmentHost
{
    int ContainerId { get; }

    FragmentManager FragmentManager { get; }
}
