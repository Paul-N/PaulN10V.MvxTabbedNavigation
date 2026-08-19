using MvvmCross.ViewModels;
using PaulN10V.MvxTabbedNavigation.Platforms.Android.Views;

namespace PaulN10V.MvxTabbedNavigation.Platforms.Android.ViewModels;

public class BottomNavigationNativeViewModelHolder : NativeViewModelHolder
{
    public TabPresentationData TabPresentationData { get; private set; }

    // ReSharper disable once ConvertToPrimaryConstructor
    public BottomNavigationNativeViewModelHolder(IMvxViewModel viewModel) : base(viewModel)
    {
        TabPresentationData = new TabPresentationData();
    }
}