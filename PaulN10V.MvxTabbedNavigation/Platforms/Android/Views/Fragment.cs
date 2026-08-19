using Android.Runtime;
using AndroidX.Lifecycle;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters;
using MvvmCross.Platforms.Android.Views;
using MvvmCross.Platforms.Android.Views.Fragments;
using MvvmCross.Platforms.Android.Views.Fragments.EventSource;
using MvvmCross.ViewModels;
using PaulN10V.MvxTabbedNavigation.Platforms.Android.ViewModels;
using JavaClass = Java.Lang.Class;

namespace PaulN10V.MvxTabbedNavigation.Platforms.Android.Views;

[Register("me.n10v.paul.tabbednavigation.platforms.android.views.Fragment")]
public class Fragment : MvxEventSourceFragment, IMvxFragmentView
{
    protected INativeViewModelHolder? ViewModelHolder;

    public string UniqueImmutableCacheTag => string.Empty; //we don't need this

    private IMvxBindingContext? _bindingContext;

    public IMvxBindingContext? BindingContext
    {
        get => _bindingContext;
        set
        {
            _bindingContext = value;
            if(_bindingContext != null)
                _bindingContext.DataContext = ViewModelHolder?.ViewModel;
        }
    }

    public object? DataContext
    {
        get => ViewModelHolder?.ViewModel;
        // ReSharper disable once ValueParameterNotUsed
        set =>
            //What should we do here? View model is constructed and assigned inside the fragment.
            Console.WriteLine($"There was attempt to set {nameof(DataContext)}. Ignoring");
    }

    public virtual IMvxViewModel? ViewModel
    {
        get => ViewModelHolder?.ViewModel;
        // ReSharper disable once ValueParameterNotUsed
        set =>
            //What should we do here? View model is constructed and assigned inside the fragment.
            Console.WriteLine($"There was attempt to set {nameof(ViewModel)}. Ignoring");
    }

    protected Fragment(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

    protected Fragment() => this.AddEventListeners();

    public override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        if (Arguments != null 
            && Arguments.GetString(MvxAndroidViewPresenter.ViewModelRequestBundleKey) is string serializedRequest
            && Mvx.IoCProvider?.TryResolve<IMvxNavigationSerializer>(out var navigationSerializer) == true
            && navigationSerializer?.Serializer is IMvxTextSerializer textSerializer)
        {
            if (textSerializer.DeserializeObject<MvxViewModelRequest>(serializedRequest) is { } vmRequest)
            {
                var vmInitializerWrapper = new ViewModelInitializerWrapper(vmRequest, ViewModelHolderType);
                var clazz = JavaClass.FromType(ViewModelHolderType);
                var myViewModel = new ViewModelProvider(this, ViewModelProvider.Factory.Companion.From(vmInitializerWrapper.GetInitializer())).Get(clazz);
                ViewModelHolder = myViewModel as INativeViewModelHolder;
            }

            OnViewModelSet();
            ViewModel?.ViewCreated();
        }
    }

    public virtual void OnViewModelSet() { }

    public override void OnDestroy()
    {
        base.OnDestroy();
        ViewModel?.ViewDestroy(viewFinishing: IsRemoving || Activity == null || Activity.IsFinishing);
    }

    public override void OnStart()
    {
        base.OnStart();
        ViewModel?.ViewAppearing();
    }

    public override void OnResume()
    {
        base.OnResume();
        ViewModel?.ViewAppeared();
    }

    public override void OnPause()
    {
        base.OnPause();
        ViewModel?.ViewDisappearing();
    }

    public override void OnStop()
    {
        base.OnStop();
        ViewModel?.ViewDisappeared();
    }

    protected virtual Type ViewModelHolderType => typeof(NativeViewModelHolder);
}

public abstract class Fragment<TViewModel> : Fragment, IMvxFragmentView<TViewModel>
    where TViewModel : class, IMvxViewModel
{
    protected Fragment() { }

    protected Fragment(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

    public new TViewModel? ViewModel
    {
        get => base.ViewModel as TViewModel;
        set => base.ViewModel = value;
    }

    public MvxFluentBindingDescriptionSet<IMvxFragmentView<TViewModel>, TViewModel> CreateBindingSet() 
        => this.CreateBindingSet<IMvxFragmentView<TViewModel>, TViewModel>();
}