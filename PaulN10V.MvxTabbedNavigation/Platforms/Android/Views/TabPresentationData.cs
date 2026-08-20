using PaulN10V.MvxTabbedNavigation.Platforms.Android.Presenters.Attributes;

namespace PaulN10V.MvxTabbedNavigation.Platforms.Android.Views;

public class TabPresentationData
{
    public List<TabPresentationAttribute> Attributes { get; } = new();
        
    private int _selectedTabIndex;

    public int SelectedTabIndex => _selectedTabIndex;

    public void SetSelectedTabIndex(int index) => _selectedTabIndex = index;

    public void AddTab(TabPresentationAttribute tabPresentationAttribute) => Attributes.Add(tabPresentationAttribute);

    public int? RemoveTabById(string tabId)
    {
        var attrToRemove = Attributes.FirstOrDefault(a => a.TabId == tabId);

        if (attrToRemove != null)
        {
            var index = Attributes.IndexOf(attrToRemove);
            Attributes.Remove(attrToRemove);
            return index;
        }

        return null;
    }
}