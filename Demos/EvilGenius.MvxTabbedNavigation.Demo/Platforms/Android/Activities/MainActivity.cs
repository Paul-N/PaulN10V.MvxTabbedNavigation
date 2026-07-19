using Android.App;
using Android.OS;
using MvvmCross.Platforms.Android.Views;
using FragmentManagerX = AndroidX.Fragment.App.FragmentManager;
using EvilGenius.MvxTabbedNavigation.Platforms.Android.Views;
using AndroidResource = EvilGenius.MvxTabbedNavigation.Demo.Resource;


// ReSharper disable once CheckNamespace
namespace EvilGenius.MvxTabbedNavigation.Demo.Platforms.Android.Activities;

[Activity(Theme = "@style/AppTheme.NoActionBar")]
public class MainActivity : MvxActivity, ISingleHostActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        // ReSharper disable once AccessToStaticMemberViaDerivedType
        SetContentView(AndroidResource.Layout.activity_main);
    }

    #region IHostViewContainer impl
    // ReSharper disable once AccessToStaticMemberViaDerivedType
    public int ContainerId => AndroidResource.Id.containerMaster;

    FragmentManagerX IFragmentHost.FragmentManager => SupportFragmentManager;

    public event EventHandler<BackPressedRequestedEventArgs> OnBackRequested;

    #endregion

#pragma warning disable CA1422
    public override void OnBackPressed()
    {
        var ea = new BackPressedRequestedEventArgs { Result = BackPressedHandlingResult.NotHandled };
            
        OnBackRequested?.Invoke(this, ea);

        switch (ea.Result)
        {
            case BackPressedHandlingResult.NotHandled:
                base.OnBackPressed();
                break;
            case BackPressedHandlingResult.Handled:
                //Do nothing
                break;
            case BackPressedHandlingResult.FinishActivity:
                Finish();
                break;
            default:
                base.OnBackPressed();
                break;
        }
    }
#pragma warning restore CA1422
}