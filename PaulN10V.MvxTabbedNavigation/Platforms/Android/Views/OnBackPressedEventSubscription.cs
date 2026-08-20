using MvvmCross.WeakSubscription;

namespace PaulN10V.MvxTabbedNavigation.Platforms.Android.Views;

internal class OnBackPressedEventSubscription : MvxWeakEventSubscription<EventSourceOnBackPressedCallback>
{
    private readonly WeakReference<EventSourceOnBackPressedCallback> _sourceReference;

    // ReSharper disable once ConvertToPrimaryConstructor
    public OnBackPressedEventSubscription(EventSourceOnBackPressedCallback source, string sourceEventName, EventHandler targetEventHandler)
        : base(source, sourceEventName, targetEventHandler) => _sourceReference = new WeakReference<EventSourceOnBackPressedCallback>(source);

    protected override void Dispose(bool disposing)
    {
        if (_sourceReference.TryGetTarget(out var source))
            source.Remove();

        base.Dispose(disposing);
    }
}