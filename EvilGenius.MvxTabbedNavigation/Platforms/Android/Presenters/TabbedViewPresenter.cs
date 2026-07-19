using Android.Content;
using AndroidX.Fragment.App;
using EvilGenius.MvxTabbedNavigation.Platforms.Android.Core;
using EvilGenius.MvxTabbedNavigation.Platforms.Android.Presenters.Attributes;
using EvilGenius.MvxTabbedNavigation.Platforms.Android.Views;
using EvilGenius.MvxTabbedNavigation.Presenters.Attributes;
using EvilGenius.MvxTabbedNavigation.Presenters.Hints;
using MvvmCross.Base;
using MvvmCross.Exceptions;
using MvvmCross.Platforms.Android.Presenters;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;
using MvvmCross.Platforms.Android.Views.Fragments;
using MvvmCross.Presenters;
using MvvmCross.Presenters.Attributes;
using MvvmCross.Presenters.Hints;
using MvvmCross.ViewModels;
using MvvmCross.WeakSubscription;
using System.Reflection;
using ActivityX = AndroidX.AppCompat.App.AppCompatActivity;
using AndroidAppActivity = Android.App.Activity;
using Fragment = AndroidX.Fragment.App.Fragment;
using FragmentManager = AndroidX.Fragment.App.FragmentManager;
using FragmentTransaction = AndroidX.Fragment.App.FragmentTransaction;

namespace EvilGenius.MvxTabbedNavigation.Platforms.Android.Presenters;

public class TabbedViewPresenter : MvxAndroidViewPresenter, IDisposable
{
    protected readonly string MasterBackStackId = "__masterBackStack__";

    protected virtual ISingleHostActivity? SingleHostActivity => CurrentActivity as ISingleHostActivity;

    protected virtual ITabbedFragment? RootFragment { get; set; }

    private IDisposable? _tabSelectedSubscription;

    private IDisposable? _tabbedStackChangedSubscription;

    private IDisposable? _activityLifecycleSubscription;

    private IDisposable? _backPressedSubscription;

    private IDisposable? _backRequestedSubscription;

    private IDisposable? _hostActivityFragmentAttachedSubscription;

    // ReSharper disable once RedundantDefaultMemberInitializer
    private int _currentTabIndex = 0;

    protected IList<ViewModelBackStackMetadata> ViewModelBackStacks { get; } = new List<ViewModelBackStackMetadata>();

    private bool _disposed;

    public TabbedViewPresenter(IEnumerable<Assembly> androidViewAssemblies, IActivityLifecycleListener activityLifecycleListener) 
        : base(androidViewAssemblies) 
    {
        _activityLifecycleSubscription = activityLifecycleListener.WeakSubscribe<IActivityLifecycleListener, ActivityLifecycleEventArgs>(
            nameof(IActivityLifecycleListener.ActivityStateChanged),
            OnActivityLifecycleListenerOnChanged);
    }

    public override void RegisterAttributeTypes()
    {
        //not gonna call base here because this Presenter work differently
        if (AttributeTypesToActionsDictionary == null)
            throw new InvalidOperationException("Cannot register attribute types on null dictionary");

        AttributeTypesToActionsDictionary.Register<RootFragmentPresentationAttribute>(ShowRootFragment, CloseRootFragment);

        AttributeTypesToActionsDictionary.Register<MvxFragmentPresentationAttribute>(ShowFragment, CloseFragment);

        AttributeTypesToActionsDictionary.Register<TabPresentationAttribute>(ShowTabFragment, CloseTabFragment);

        AttributeTypesToActionsDictionary.Register<OverTopPresentationAttribute>(ShowFragmentOverTop, CloseFragmentOverTop);
    }


    private async Task<bool> ShowRootFragment(Type type, RootFragmentPresentationAttribute attr, MvxViewModelRequest vmRequest)
    {
        PendingRequest = vmRequest;

        if (attr.HostActivityType == SingleHostActivity?.GetType())
        {
            await ShowRootFragment(vmRequest, SingleHostActivity as AndroidAppActivity);
            return true;
        }
        else
        {

            var showActivityRes = await this.ShowHostActivity(attr.HostActivityType);

            if (!showActivityRes)
            {
                PendingRequest = null;
                return false;
            }

            return true;
        }
    }

    private Task<bool> ShowRootFragment(MvxViewModelRequest vmRequest, AndroidAppActivity? activity)
    {
        if (activity is ISingleHostActivity { FragmentManager: FragmentManager fm, ContainerId: var containerId })
        {
            MvxFragmentPresentationAttribute presentationAttr = new MvxFragmentPresentationAttribute
            {
                AddFragment = false,
                AddToBackStack = true,
                AllowReordering = true,
                // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
                ViewType = ViewsContainer.GetViewType(vmRequest?.ViewModelType),
                FragmentContentId = containerId,
                Tag = MasterBackStackId
            };

            PerformShowFragmentTransaction(fm, presentationAttr, vmRequest!);
        }
        else
            return Task.FromResult(false);

        return Task.FromResult(true);
    }

    private void OnTabbedFragmentAttached(object? sender, FragmentOnAttachEventArgs ea)
    {
        if (ea.P1 is ITabbedFragment tabbedFragment)
        {
            this.RootFragment = tabbedFragment;
            _tabSelectedSubscription = tabbedFragment.WeakSubscribe<ITabbedFragment, MvxValueEventArgs<int>>(nameof(ITabbedFragment.TabSelected), OnTabSelected);

            if (tabbedFragment.FragmentManager is FragmentManager fm)
                _tabbedStackChangedSubscription = fm.WeakSubscribe(nameof(FragmentManager.BackStackChanged), OnTabbedFragmentBackStackChanged);
        }
    }

    protected virtual Task<bool> ShowHostActivity(Type activityType)
    {
        var intent = new Intent(Application.Context, activityType);

        ShowIntent(intent, null);
        return Task.FromResult(true);
    }

    private Task<bool> CloseRootFragment(IMvxViewModel viewModel, RootFragmentPresentationAttribute attribute)
    {
        if (!IsTopMostFragmentRoot() || SingleHostActivity?.FragmentManager is not FragmentManager fm)
            return Task.FromResult(false);
            
        fm.PopBackStack();

        //ClearTabbedFragmentAttachedListener();

        _tabSelectedSubscription?.Dispose();
        _tabSelectedSubscription = null;

        _tabbedStackChangedSubscription?.Dispose();
        _tabbedStackChangedSubscription = null;

        _currentTabIndex = 0;

        RootFragment = null;

        ViewModelBackStacks.Clear();

        return Task.FromResult(true);

    }

    protected override Task<bool> ShowFragment(Type view, MvxFragmentPresentationAttribute attribute, MvxViewModelRequest vmRequest)
    {
        //not gonna call base here because this Presenter has different logic
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        if(vmRequest?.ViewModelType == null)
            throw new ArgumentNullException($"Either {nameof(vmRequest)} or {nameof(vmRequest)}.{nameof(vmRequest.ViewModelType)} is null");

        var result = false;

        if (IsTopMostFragmentRoot() && GetRootFragmentBackStackId() is string tabId)
        {
            ShowTabFragmentCore(attribute, vmRequest, tabId);

            ViewModelBackStacks.PushToBackStack(tabId, vmRequest.ViewModelType);
                
            result = true;
        }
        else if(SingleHostActivity is { FragmentManager: FragmentManager fm, ContainerId: var containerId })
        {
            MvxFragmentPresentationAttribute presentationAttr = new MvxFragmentPresentationAttribute
            {
                AddFragment = false,
                AddToBackStack = true,
                AllowReordering = true,
                ViewType = view,
                FragmentContentId = containerId,
                Tag = MasterBackStackId
            };

            PerformShowFragmentTransaction(fm, presentationAttr, vmRequest);
                
            result = true;
        }

        return Task.FromResult(result);
    }

    private Task<bool> ShowTabFragment(Type type, TabPresentationAttribute attribute, MvxViewModelRequest vmRequest)
    {
        if (RootFragment == null)
            throw new InvalidOperationException("Unable to show tab because there no tab fragment");

        if (vmRequest.ViewModelType == null)
            throw new ArgumentNullException($"Either {nameof(vmRequest)} or {nameof(vmRequest)}.{nameof(vmRequest.ViewModelType)} is null");

        RootFragment.AddTab(attribute);

        var isPending = ViewModelBackStacks.Count > 0;

        ViewModelBackStackMetadata metadata = isPending 
            ? ViewModelBackStackMetadata.CreateFromPendingRequest(attribute, vmRequest) 
            : ViewModelBackStackMetadata.Create(attribute.TabId, vmRequest.ViewModelType!);

        ViewModelBackStacks.Add(metadata);

        if (!isPending)
            ShowTabFragmentCore(attribute, vmRequest, attribute.TabId);

        return Task.FromResult(true);
    }

    private void ShowTabFragmentCore(MvxBasePresentationAttribute attribute, MvxViewModelRequest vmRequest, string tabId)
    {
        if (RootFragment is { FragmentManager: FragmentManager fm, ContainerId: var containerId }
            && attribute.ViewType is Type viewType)
        {
            var fragmentView = CreateFragment(fm, attribute, viewType);
            var fragment = fragmentView.ToFragment();
            if (fragment == null)
            {
                var fragmentName = viewType.FragmentJavaName();
                throw new MvxException($"Fragment {fragmentName} is null. Cannot perform Fragment Transaction.");
            }

            // MvxNavigationService provides an already instantiated ViewModel here
            //if (vmRequest is MvxViewModelInstanceRequest instanceRequest)
            //{
            //    fragmentView!.ViewModel = instanceRequest.ViewModelInstance;
            //}

            // save MvxViewModelRequest in the Fragment's Arguments
#pragma warning disable CA2000 // Dispose objects before losing scope
            var bundle = new Bundle();
#pragma warning restore CA2000 // Dispose objects before losing scope
            var serializedRequest = NavigationSerializer.Serializer.SerializeObject(vmRequest);
            bundle.PutString(ViewModelRequestBundleKey, serializedRequest);

            if (fragment.Arguments == null)
            {
                fragment.Arguments = bundle;
            }
            else
            {
                fragment.Arguments.Clear();
                fragment.Arguments.PutAll(bundle);
            }

            var ft = fm.BeginTransaction();
            ft.SetReorderingAllowed(true);
            ft.Replace(containerId, fragment);
            ft.AddToBackStack(tabId);
            ft.Commit();
        }
    }


    private Task<bool> CloseTabFragment(IMvxViewModel viewModel, TabPresentationAttribute attribute)
    {
        if (IsTopMostFragmentRoot() && RootFragment?.FragmentManager is FragmentManager fm)
        {
            var tabId = attribute.TabId;

            fm.SaveBackStack(tabId);
            fm.ClearBackStack(tabId);

            if (ViewModelBackStacks.Count != 0)
            {

                var isNextTabToSelect = _currentTabIndex + 1 < ViewModelBackStacks.Count;

                var tabToSelect = isNextTabToSelect ? _currentTabIndex + 1 : _currentTabIndex - 1;

                ChangeCurrentBackStack(tabToSelect);

                RootFragment.RemoveTab(tabId);

                if (isNextTabToSelect)
                    _currentTabIndex--;
            }
            else
                _currentTabIndex = 0;

            ViewModelBackStacks.RemoveBackStack(tabId);

            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    private Task<bool> ShowFragmentOverTop(Type type, OverTopPresentationAttribute attribute, MvxViewModelRequest vmRequest)
    {
        if (SingleHostActivity is { FragmentManager: FragmentManager fm, ContainerId: var containerId }
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            && vmRequest != null)
        {
            MvxFragmentPresentationAttribute presentationAttr = new MvxFragmentPresentationAttribute
            {
                AddFragment = false,
                AddToBackStack = true,
                AllowReordering = true,
                ViewType = type,
                FragmentContentId = containerId,
                Tag = MasterBackStackId
            };

            PerformShowFragmentTransaction(fm, presentationAttr, vmRequest);
        }

        return Task.FromResult(false);
    }

    private Task<bool> CloseFragmentOverTop(IMvxViewModel viewModel, OverTopPresentationAttribute attribute)
    {
        if (SingleHostActivity?.ContainerId is int containerId)
        {
            MvxFragmentPresentationAttribute presentationAttr = new MvxFragmentPresentationAttribute
            {
                AddFragment = false,
                AddToBackStack = false,
                ViewType = attribute.ViewType,
                //FragmentHostViewType = this.SingleHostActivity?.GetType(),
                FragmentContentId = containerId
            };

            return CloseFragment(viewModel, presentationAttr);
        }
        else
            return Task.FromResult(false);
    }

    protected override void OnBeforeFragmentChanging(FragmentTransaction fragmentTransaction, Fragment fragment, MvxFragmentPresentationAttribute attribute, MvxViewModelRequest request)
    {
        base.OnBeforeFragmentChanging(fragmentTransaction, fragment, attribute, request);

        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        fragmentTransaction?.SetReorderingAllowed(true);
    }

    protected override void OnFragmentChanged(FragmentTransaction? fragmentTransaction, Fragment? fragment, MvxFragmentPresentationAttribute? attribute, MvxViewModelRequest? request)
    {
        base.OnFragmentChanged(fragmentTransaction, fragment, attribute, request);

        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        var viewType = ViewsContainer?.GetViewType(request?.ViewModelType);
        if (viewType?.HasBasePresentationAttribute() == true && viewType.GetBasePresentationAttributes().Any(pa => pa is RootFragmentPresentationAttribute))
        {
            fragmentTransaction?.SetPrimaryNavigationFragment(fragment);
            //RootFragment = fragment as ITabbedFragment;
        }
    }

    public override MvxBasePresentationAttribute CreatePresentationAttribute(Type? viewModelType, Type? viewType)
    {
        return new MvxFragmentPresentationAttribute(GetCurrentActivityViewModelType(), global::Android.Resource.Id.Content)
        {
            ViewType = viewType,
            ViewModelType = viewModelType
        };
    }

    protected virtual void OnTabSelected(object? sender, MvxValueEventArgs<int> ea) => ChangeCurrentBackStack(ea.Value);

    protected virtual void ChangeCurrentBackStack(int backStackIndexToChange)
    {
        var fm = RootFragment?.FragmentManager;
        fm?.SaveBackStack(ViewModelBackStacks[_currentTabIndex].TabId);

        var tabDataToSelect = ViewModelBackStacks[backStackIndexToChange];

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (tabDataToSelect != null)
        {
            if (tabDataToSelect.IsPending)
            {
                (TabPresentationAttribute pAttr, MvxViewModelRequest vmReq) = tabDataToSelect.ExtractPendingRequestData();
                ShowTabFragmentCore(pAttr, vmReq, pAttr.TabId);
            }
            else
                fm?.RestoreBackStack(tabDataToSelect.TabId);
        }

        _currentTabIndex = backStackIndexToChange;
    }

    private void OnTabbedFragmentBackStackChanged(object? sender, EventArgs e)
    {
        if (RootFragment is { FragmentManager: { BackStackEntryCount: > 0 } fm } && fm.GetBackStackEntryAt(0) is { Name: string tabId })
        {
            var entryCount = fm.BackStackEntryCount;
            ViewModelBackStacks.PopFromBackStackIfNeeded(tabId, entryCount);
        }
    }

    protected override void ActivityLifetimeListenerOnActivityChanged(object? sender, MvxActivityEventArgs ea)
    {
        if (ea.ActivityState == MvxActivityState.OnResume && PendingRequest?.ViewModelType is Type vmType)
        {
            var viewType = ViewsContainer.GetViewType(vmType);
            if (viewType?.GetBasePresentationAttributes().Any(pa => pa is RootFragmentPresentationAttribute) == true)
            {
                ShowRootFragment(PendingRequest, ea.Activity);
                PendingRequest = null;
                return;
            }
        }
        base.ActivityLifetimeListenerOnActivityChanged(sender, ea);
    }

    protected virtual void OnActivityLifecycleListenerOnChanged(object? sender, ActivityLifecycleEventArgs ea)
    {
        if (ea.Activity is ISingleHostActivity hostActivity)
        {
            if (ea.ActivityState == MvxActivityState.OnCreate)
            {
                if (hostActivity.FragmentManager is FragmentManager fm)
                    _hostActivityFragmentAttachedSubscription = fm.WeakSubscribe<FragmentManager, FragmentOnAttachEventArgs>(nameof(FragmentManager.FragmentOnAttach), OnTabbedFragmentAttached);

                var backPressedCallback = new EventSourceOnBackPressedCallback(true);
                _backPressedSubscription = new OnBackPressedEventSubscription(
                    backPressedCallback, 
                    nameof(EventSourceOnBackPressedCallback.OnBackPressed), 
                    OnHostActivityBackPressed);

                if (ea.Activity is ActivityX activity)
                    activity.OnBackPressedDispatcher.AddCallback(backPressedCallback);

                _backRequestedSubscription = hostActivity.WeakSubscribe<IBackPressedAware, BackPressedRequestedEventArgs>(nameof(IBackPressedAware.OnBackRequested), OnHostActivityBackPressed);
            }

            if (ea.ActivityState == MvxActivityState.OnDestroy)
            {
                ClearHostActivitySubscriptions();
            }
        }
    }

    public override Task<bool> ChangePresentation(MvxPresentationHint hint) => hint switch
    {
        null => throw new ArgumentNullException(nameof(hint)),
        MvxPagePresentationHint pagePresentationHint => Task.FromResult(ChangePagePresentation(pagePresentationHint)),
        ClearStackPresentationHint => Task.FromResult(ClearBackStack()),
        MvxPopToRootPresentationHint => Task.FromResult(PopToRoot()),
        _ => base.ChangePresentation(hint),
    };

    private bool ChangePagePresentation(MvxPagePresentationHint pagePresentationHint)
    {
        for (int i = 0; i<ViewModelBackStacks.Count; i++)
        {
            var metaDataEntry = ViewModelBackStacks[i];
            if (metaDataEntry.HasViewModelInStack(pagePresentationHint.ViewModel))
            {
                OnTabSelected(null, new MvxValueEventArgs<int>(i));
                RootFragment?.SelectTabAt(i);
                return true;
            }
        }
        return false;
    }

    private bool ClearBackStack()
    {
        if (IsTopMostFragmentRoot() && GetRootFragmentBackStackId() is string tabId)
        {
            var fm = RootFragment?.FragmentManager;
                
            fm?.SaveBackStack(tabId);
            fm?.ClearBackStack(tabId);

            this.ViewModelBackStacks.ClearBackStack(tabId);

            return true;
        }
        else if (SingleHostActivity?.FragmentManager is FragmentManager fm && GetHostActivityBackStackId() is string stackId)
        {
            fm.SaveBackStack(stackId);
            fm.ClearBackStack(stackId);
            return true;
        }
        return false;
    }

    private bool PopToRoot()
    {
        if (IsTopMostFragmentRoot() && GetRootFragmentBackStackId() is string tabId && GetFirstFragmentIdInRootFrag() is int firstIdInRoot)
        {
            RootFragment!.FragmentManager.PopBackStack(firstIdInRoot, (int)PopBackStackFlags.Inclusive);
                
            this.ViewModelBackStacks.PopToRoot(tabId);

            return true;
        }
        else if (GetFirstFragmentIdInHost() is int firstIdInHost)
        {
            SingleHostActivity!.FragmentManager.PopBackStack(firstIdInHost, (int)PopBackStackFlags.Inclusive);

            return true;
        }
        return false;
    }

    private void ClearHostActivitySubscriptions()
    {
        _hostActivityFragmentAttachedSubscription?.Dispose();

        _hostActivityFragmentAttachedSubscription = null;

        _backPressedSubscription?.Dispose();

        _backPressedSubscription = null;

        _backRequestedSubscription?.Dispose();

        _backRequestedSubscription = null;
    }

    protected bool IsTopMostFragmentRoot() => SingleHostActivity?.FragmentManager?.Fragments.Last() == RootFragment && RootFragment != null;

    protected string? GetRootFragmentBackStackId() => GetBackStackId(RootFragment?.FragmentManager);

    protected string GetHostActivityBackStackId() => MasterBackStackId;

    protected string? GetBackStackId(FragmentManager? fm) =>
        fm?.BackStackEntryCount > 0
        && fm.GetBackStackEntryAt(fm.BackStackEntryCount - 1) is { Name: string backStackId }
            ? backStackId
            : null;

    protected bool IsLastFragmentPresentedInRootFragment() => IsLastFragmentPresented(RootFragment?.FragmentManager);

    protected bool IsLastFragmentPresentedInHostActivity() => IsLastFragmentPresented(SingleHostActivity?.FragmentManager);

    protected bool IsLastFragmentPresented(FragmentManager? fm) => !(fm?.BackStackEntryCount > 1);

    protected int? GetFirstFragmentIdInRootFrag() => GetFirstFragmentIdInStackStack(RootFragment?.FragmentManager);

    protected int? GetFirstFragmentIdInHost() => GetFirstFragmentIdInStackStack(SingleHostActivity?.FragmentManager);

    private int? GetFirstFragmentIdInStackStack(FragmentManager? fragmentManager)
    {
        //if (fragmentManager != null)
        //{
        //    var ids = Enumerable.Range(0, fragmentManager.BackStackEntryCount).Select(i => { var bse = fragmentManager.GetBackStackEntryAt(i);
        //        //return (bse.Id, bse.Name);
        //        return bse.Id;
        //    }).ToArray();
        //}
        return fragmentManager is { BackStackEntryCount: > 1 } && fragmentManager.GetBackStackEntryAt(1).Id is var id
            ? id
            : null;
    }

    protected virtual void OnHostActivityBackPressed(object? sender, EventArgs e) { }

    protected virtual void OnHostActivityBackPressed(object? sender, BackPressedRequestedEventArgs ea) 
    {
        if (IsTopMostFragmentRoot())
        {
            ea.Result = IsLastFragmentPresentedInRootFragment() ? BackPressedHandlingResult.FinishActivity : BackPressedHandlingResult.NotHandled;
        }
        else
        {
            ea.Result = BackPressedHandlingResult.NotHandled;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                ClearHostActivitySubscriptions();
                _tabSelectedSubscription?.Dispose();
                _tabSelectedSubscription = null;
                _tabbedStackChangedSubscription?.Dispose();
                _tabbedStackChangedSubscription = null;
                _activityLifecycleSubscription?.Dispose();
                _activityLifecycleSubscription = null;
            }

            _disposed = true;
        }
    }

    ~TabbedViewPresenter() => Dispose(disposing: false);

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}