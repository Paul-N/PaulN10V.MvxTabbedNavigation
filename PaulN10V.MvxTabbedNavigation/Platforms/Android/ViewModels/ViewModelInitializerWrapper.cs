using AndroidX.Lifecycle;
using AndroidX.Lifecycle.ViewModels;
using Java.Lang;
using Microsoft.Extensions.Logging;
using MvvmCross;
using MvvmCross.Logging;
using MvvmCross.ViewModels;

namespace PaulN10V.MvxTabbedNavigation.Platforms.Android.ViewModels;

public class ViewModelInitializerWrapper
{
    readonly MvxViewModelRequest _vmRequest;
    private readonly Type _nativeVmHolderType;
    private readonly Class _class;
    private readonly Func<CreationExtras, ViewModel?> _generatorFunc;

    public ViewModelInitializerWrapper(MvxViewModelRequest vmRequest, Type nativeVmHolderType)
    {
        _vmRequest = vmRequest;
        _nativeVmHolderType = nativeVmHolderType;
        _class = Class.FromType(_nativeVmHolderType);
        _generatorFunc = Create;
    }

    public ViewModelInitializer GetInitializer() => new(_class, _generatorFunc.Kotlinize());

    private ViewModel? Create(CreationExtras creationExtras)
    {
        var vm = LoadViewModelFrom(_vmRequest);
        return Activator.CreateInstance(_nativeVmHolderType, vm) as ViewModel;
    }

    //Stolen from \Mvx9\MvvmCross\Platforms\Android\Views\Fragments\MvxFragmentExtensions.cs
    private IMvxViewModel? LoadViewModelFrom(MvxViewModelRequest request)
    {
        var loader = Mvx.IoCProvider?.Resolve<IMvxViewModelLoader>();
        var viewModel = loader?.LoadViewModel(request, null);
        if (viewModel == null)
        {
            MvxLogHost.Default?.Log(LogLevel.Warning, "ViewModel not loaded for {viewModelType}", request.ViewModelType?.FullName);
        }
        return viewModel;
    }
}