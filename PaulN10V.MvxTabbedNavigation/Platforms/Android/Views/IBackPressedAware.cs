namespace PaulN10V.MvxTabbedNavigation.Platforms.Android.Views;

public interface IBackPressedAware
{
    event EventHandler<BackPressedRequestedEventArgs>? OnBackRequested;
}