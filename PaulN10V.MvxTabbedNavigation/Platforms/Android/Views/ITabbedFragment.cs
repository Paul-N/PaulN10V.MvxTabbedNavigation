using MvvmCross.Base;
using PaulN10V.MvxTabbedNavigation.Platforms.Android.Presenters.Attributes;

namespace PaulN10V.MvxTabbedNavigation.Platforms.Android.Views;

public interface ITabbedFragment : IFragmentHost
{
    void AddTab(TabPresentationAttribute tabPresentationAttribute);

    event EventHandler<MvxValueEventArgs<int>> TabSelected;

    void SelectTabAt(int index);

    void RemoveTab(string tabId);

    //void UpdateCurrentTabTitle(FragNavTabPresentationAttribute tabPresentationAttribute, int tabIndex);
}