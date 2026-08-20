using MvvmCross.Presenters.Attributes;

namespace PaulN10V.MvxTabbedNavigation.Platforms.Android.Presenters.Attributes;

public class TabPresentationAttribute : MvxBasePresentationAttribute
{
    public string TabId { get; set; } = null!;

    public string? TabTitle { get; set; }

    public string? TabTitleCondensed { get; set; }

    public int TabTitleResourceId { get; set; }

    public int TabTitleCondensedResourceId { get; set; }

    public int IconResourceId { get; set; }
}