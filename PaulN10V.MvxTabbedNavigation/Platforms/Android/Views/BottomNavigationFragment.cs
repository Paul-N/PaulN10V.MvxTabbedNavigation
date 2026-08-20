using Android.Views;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.Navigation;
using MvvmCross.Base;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Views;
using MvvmCross.ViewModels;
using PaulN10V.MvxTabbedNavigation.Platforms.Android.Presenters.Attributes;
using PaulN10V.MvxTabbedNavigation.Platforms.Android.ViewModels;
using FragmentManager = AndroidX.Fragment.App.FragmentManager;

namespace PaulN10V.MvxTabbedNavigation.Platforms.Android.Views;

public abstract class BottomNavigationFragment : Fragment, ITabbedFragment
{
    protected TabPresentationData? TabPresentationData => (ViewModelHolder as BottomNavigationNativeViewModelHolder)?.TabPresentationData;
        
    private BottomNavigationView? _bottomNavigationView;

    public abstract int ContainerId { get; }

    public abstract int BottomNavigationViewId { get; }

    FragmentManager IFragmentHost.FragmentManager => ChildFragmentManager;


    public event EventHandler<MvxValueEventArgs<int>>? TabSelected;

    public void AddTab(TabPresentationAttribute tabPresentationAttribute) => AddTabCore(tabPresentationAttribute);

    protected void AddTabCore(TabPresentationAttribute tabPresentationAttribute, bool shouldAddToVmHolder = true)
    {
        var navView = _bottomNavigationView;
        IMenuItem? menuItem;
        int itemId = View.GenerateViewId();

        if (!string.IsNullOrEmpty(tabPresentationAttribute.TabTitle))
            menuItem = navView?.Menu.Add(IMenu.None, itemId, IMenu.None, tabPresentationAttribute.TabTitle);
        else if (tabPresentationAttribute.TabTitleResourceId != IMenu.None)
            menuItem = navView?.Menu.Add(IMenu.None, itemId, IMenu.None, tabPresentationAttribute.TabTitleResourceId);
        else
            menuItem = navView?.Menu.Add(IMenu.None, itemId, IMenu.None, string.Empty);

        if (!string.IsNullOrEmpty(tabPresentationAttribute.TabTitleCondensed))
            menuItem?.SetTitleCondensed(tabPresentationAttribute.TabTitleCondensed);
        else if (tabPresentationAttribute.TabTitleCondensedResourceId != IMenu.None)
            menuItem?.SetTitleCondensed(Context?.Resources?.GetString(tabPresentationAttribute.TabTitleCondensedResourceId));

        if (tabPresentationAttribute.IconResourceId != IMenu.None)
            menuItem?.SetIcon(tabPresentationAttribute.IconResourceId);
            
        if(shouldAddToVmHolder)
            TabPresentationData?.AddTab(tabPresentationAttribute);
    }

    public void SelectTabAt(int index)
    {
        if(_bottomNavigationView?.Menu.GetItem(index) is { } item)
            _bottomNavigationView!.SelectedItemId = item.ItemId;

        TabPresentationData?.SetSelectedTabIndex(index);
    }

    public void RemoveTab(string tabId)
    {
        var indexToRemove = TabPresentationData?.RemoveTabById(tabId);

        if (indexToRemove is int index)
        {
            var menu = _bottomNavigationView?.Menu;

            if (menu?.GetItem(index) is IMenuItem menuItem)
                menu.RemoveItem(menuItem.ItemId);
        }
    }

    private void OnBottomBarNavigationItemSelected(object? sender, NavigationBarView.ItemSelectedEventArgs ea)
    {
        for (int i = 0; i < _bottomNavigationView?.Menu.Size(); i++)
            if (_bottomNavigationView.Menu.GetItem(i) == ea.P0)
            {
                TabPresentationData?.SetSelectedTabIndex(i);
                TabSelected?.Invoke(sender, new MvxValueEventArgs<int>(i));
            }
    }

    protected override Type ViewModelHolderType => typeof(BottomNavigationNativeViewModelHolder);

    public override void OnViewCreated(View view, Bundle? savedInstanceState)
    {
        base.OnViewCreated(view, savedInstanceState);

        _bottomNavigationView = View?.FindViewById<BottomNavigationView>(BottomNavigationViewId);
        if (_bottomNavigationView != null)
        {
            _bottomNavigationView.ItemSelected += OnBottomBarNavigationItemSelected;
        }
        else throw new Exception($"Unable to find navigation view. Check {nameof(BottomNavigationViewId)}");

        if (TabPresentationData is TabPresentationData tabPresentationData && tabPresentationData.Attributes.Any())
        {
            foreach (var tabAttr in tabPresentationData.Attributes)
                AddTabCore(tabAttr, false);

            this.SelectTabAt(tabPresentationData.SelectedTabIndex);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if(_bottomNavigationView != null)
            _bottomNavigationView.ItemSelected -= OnBottomBarNavigationItemSelected;
            
        _bottomNavigationView = null;
            
        base.Dispose(disposing);
    }
}

public abstract class BottomNavigationFragment<TViewModel> : BottomNavigationFragment, IMvxFragmentView<TViewModel>
    where TViewModel : class, IMvxViewModel
{
    public new TViewModel? ViewModel
    {
        get => base.ViewModel as TViewModel;
        set => base.ViewModel = value;
    }

    public MvxFluentBindingDescriptionSet<IMvxFragmentView<TViewModel>, TViewModel> CreateBindingSet()
    {
        return this.CreateBindingSet<IMvxFragmentView<TViewModel>, TViewModel>();
    }
}