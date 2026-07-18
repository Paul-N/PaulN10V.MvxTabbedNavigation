

using System;
using AndroidX.AppCompat.App;
using MvvmCross;

namespace EvilGenius.MvxTabbedNavigation.Platforms.Android.Views;

[Preserve(AllMembers = true)]
public interface ISingleHostActivity : IFragmentHost, IBackPressedAware { }

public class LinkerPleaseInclude
{
    public static void Inclde(ISingleHostActivity activity)
    {
        int i = activity.ContainerId;
        Console.WriteLine((i+1).ToString());
    }
}