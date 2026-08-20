namespace PaulN10V.MvxTabbedNavigation.Platforms.Android.Core;

//IMvxAndroidActivityLifetimeListener events are rising too late so we have to introduce our own listener
public interface IActivityLifecycleListener
{
    public event EventHandler<ActivityLifecycleEventArgs> ActivityStateChanged;

    public void OnActivityCreated(Activity activity, Bundle? savedInstanceState);

    public void OnActivityDestroyed(Activity activity);

    public void OnActivityPaused(Activity activity);

    public void OnActivityResumed(Activity activity);

    public void OnActivitySaveInstanceState(Activity activity, Bundle outState);

    public void OnActivityStarted(Activity activity);

    public void OnActivityStopped(Activity activity);
}