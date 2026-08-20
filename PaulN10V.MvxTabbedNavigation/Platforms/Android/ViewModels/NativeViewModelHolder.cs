using AndroidX.Lifecycle;
using MvvmCross.Base;
using MvvmCross.ViewModels;

namespace PaulN10V.MvxTabbedNavigation.Platforms.Android.ViewModels;

public class NativeViewModelHolder : ViewModel, INativeViewModelHolder //Should we implement ILifecycleObserver here?
{
    public IMvxViewModel? ViewModel { get; private set; }

    // ReSharper disable once ConvertToPrimaryConstructor
    public NativeViewModelHolder(IMvxViewModel viewModel) => ViewModel = viewModel;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            ViewModel?.DisposeIfDisposable();
            
        ViewModel = null;
            
        base.Dispose(disposing);
    }
}