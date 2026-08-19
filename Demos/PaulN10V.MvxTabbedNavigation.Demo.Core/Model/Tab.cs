namespace PaulN10V.MvxTabbedNavigation.Demo.Core.Model;

public enum Tab : byte
{
    TabOne, TabTwo, TabThree, TabSecure, TabLogin
}

public static class TabNames
{
    public const string TabOne = nameof(Tab.TabOne);

    public const string TabTwo = nameof(Tab.TabTwo);

    public const string TabThree = nameof(Tab.TabThree);

    public const string TabSecure = nameof(Tab.TabSecure);

    public const string TabLogin = nameof(Tab.TabLogin);
}