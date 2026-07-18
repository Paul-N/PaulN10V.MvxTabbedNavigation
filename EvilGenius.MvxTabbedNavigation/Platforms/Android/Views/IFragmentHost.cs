using FragmentManager = AndroidX.Fragment.App.FragmentManager;

namespace EvilGenius.MvxTabbedNavigation.Platforms.Android.Views;

public interface IFragmentHost
{
    int ContainerId { get; }

    FragmentManager FragmentManager { get; }
}
