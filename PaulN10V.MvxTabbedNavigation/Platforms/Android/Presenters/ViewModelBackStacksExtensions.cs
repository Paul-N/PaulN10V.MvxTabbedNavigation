namespace PaulN10V.MvxTabbedNavigation.Platforms.Android.Presenters;

public static class ViewModelBackStacksExtensions
{
    public static void PushToBackStack(this IList<ViewModelBackStackMetadata> viewModelBackStacks, string tabId, Type viewModelType)
    {
        var metadata = viewModelBackStacks.SingleOrDefault(s => s.TabId == tabId);
        metadata?.Push(viewModelType);
    }

    public static void PopFromBackStack(this IList<ViewModelBackStackMetadata> viewModelBackStacks, string tabId)
    {
        var metadata = viewModelBackStacks.SingleOrDefault(s => s.TabId == tabId);
        metadata?.Pop();
    }

    public static void PopFromBackStackIfNeeded(this IList<ViewModelBackStackMetadata> viewModelBackStacks, string tabId, int count)
    {
        var metadata = viewModelBackStacks.SingleOrDefault(s => s.TabId == tabId);
        metadata?.PopIfNeeded(count);
    }

    public static void ClearBackStack(this IList<ViewModelBackStackMetadata> viewModelBackStacks, string tabId)
    {
        var metadata = viewModelBackStacks.SingleOrDefault(s => s.TabId == tabId);
        metadata?.Clear();
    }

    public static void PopToRoot(this IList<ViewModelBackStackMetadata> viewModelBackStacks, string tabId)
    {
        var metadata = viewModelBackStacks.SingleOrDefault(s => s.TabId == tabId);
        metadata?.PopToRoot();
    }

    public static void RemoveBackStack(this IList<ViewModelBackStackMetadata> viewModelBackStacks, string tabId)
    {
        if(viewModelBackStacks.FirstOrDefault(s => s.TabId == tabId) is ViewModelBackStackMetadata metadata)
            viewModelBackStacks.Remove(metadata);
    }
}