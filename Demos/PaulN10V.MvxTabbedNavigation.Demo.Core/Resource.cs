using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace PaulN10V.MvxTabbedNavigation.Demo.Core;

public static class Resource
{
    private static readonly ISet<Color> Colors;

    private static int _currentColorIdx;

    static Resource()
    {
        var strColors = new[]
        {
            "#003f5c", "#2f4b7c", "#665191", "#a05195", "#d45087", "#f95d6a", "#ff7c43", "#ffa600"
        };

        var converter = TypeDescriptor.GetConverter(typeof(Color));

        Colors = new HashSet<Color>(strColors.Select(s => (Color)converter.ConvertFromString(s)!));
    }

    public const string Account = "Account";

    public static string AccountInfo => "Account info";

    public static string OpenNew => "Open new";

    public static string OpenOverTop => "Open over the top";

    public static string PopToRoot => "Pop to root";

    public static string Close => "Close";

    public static string GoLogin => "Go login";

    public static string EnterPhone => "Enter your phone";

    public static string GetCode => "Get code";

    public static string EnterSms => "Enter SMS code";

    public static string Send => "Send";

    public static string Thanks => "Thanks for trying this app!";

    public static string Start => "Start";

    public static readonly int _44px = 44;

    public static readonly int TitleWidth = 200;

    public const string SecureTab = "Secure";

    public const string OneTab = "ONE";

    public const string TwoTab = "TWO";

    public const string ThreeTab = "THREE";

    public const string LoginTab = "Login";

    public static Color GetRandomColor()
    {
        var color = Colors.ElementAt(_currentColorIdx);
        _currentColorIdx = _currentColorIdx == Colors.Count - 1 ? 0 : _currentColorIdx + 1;
        return color;
    }

}