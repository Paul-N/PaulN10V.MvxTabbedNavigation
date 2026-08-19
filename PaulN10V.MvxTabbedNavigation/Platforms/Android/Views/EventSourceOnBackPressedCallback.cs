using AndroidX.Activity;

namespace PaulN10V.MvxTabbedNavigation.Platforms.Android.Views;

internal class EventSourceOnBackPressedCallback : OnBackPressedCallback
{
    public event EventHandler OnBackPressed = null!;

    // ReSharper disable once ConvertToPrimaryConstructor
    public EventSourceOnBackPressedCallback(bool enabled) : base(enabled) { }

    public override void HandleOnBackPressed() => OnBackPressed?.Invoke(this, EventArgs.Empty);
}