using MvvmCross.Platforms.Android.Views;

namespace PaulN10V.MvxTabbedNavigation.Platforms.Android.Core;

public class ActivityLifecycleListener : Java.Lang.Object, Application.IActivityLifecycleCallbacks, IActivityLifecycleListener
{
    public event EventHandler<ActivityLifecycleEventArgs>? ActivityStateChanged;

    public void OnActivityCreated(Activity activity, Bundle? savedInstanceState)
        => RaiseStateChanged(activity, MvxActivityState.OnCreate, savedInstanceState);

    public void OnActivityDestroyed(Activity activity)
        => RaiseStateChanged(activity, MvxActivityState.OnDestroy);

    public void OnActivityPaused(Activity activity)
        => RaiseStateChanged(activity, MvxActivityState.OnPause);

    public void OnActivityResumed(Activity activity)
        => RaiseStateChanged(activity, MvxActivityState.OnResume);

    public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        => RaiseStateChanged(activity, MvxActivityState.OnSaveInstanceState, outState);

    public void OnActivityStarted(Activity activity)
        => RaiseStateChanged(activity, MvxActivityState.OnStart);

    public void OnActivityStopped(Activity activity)
        => RaiseStateChanged(activity, MvxActivityState.OnStop);

    private void RaiseStateChanged(Activity activity, MvxActivityState state, Bundle? bundle = null) 
        => ActivityStateChanged?.Invoke(activity, new ActivityLifecycleEventArgs(activity, state, bundle));
}