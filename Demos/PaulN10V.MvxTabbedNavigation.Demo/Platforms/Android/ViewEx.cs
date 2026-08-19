using Android.Content.Res;
using Android.Util;
using Android.Views;
using Android.Widget;
using View = Android.Views.View;
using CoreResource = PaulN10V.MvxTabbedNavigation.Demo.Core.Resource;

// ReSharper disable once CheckNamespace
namespace PaulN10V.MvxTabbedNavigation.Demo.Platforms.Android;

internal static class ViewEx
{
    public static void SetSizeOf(this View view, int viewIdToSize, Resources res, int dpWidth, int dpHeight)
    {
        if (view.FindViewById(viewIdToSize) is View viewToSetParams &&
            viewToSetParams.LayoutParameters is ViewGroup.LayoutParams lp)
        {
            lp.Width = Convert.ToInt32(TypedValue.ApplyDimension(ComplexUnitType.Dip, dpWidth, res.DisplayMetrics));
            lp.Height = Convert.ToInt32(TypedValue.ApplyDimension(ComplexUnitType.Dip, dpHeight, res.DisplayMetrics));
        }
    }

    public static void SetTextTo(this View view, int textViewId, string title)
    {
        if(view.FindViewById<TextView>(textViewId) is TextView tv)
            tv.Text = title;
    }

    public static void SetupTitledTextView(this View view, int viewId, Resources res, string title)
    {
        if (view.FindViewById<TextView>(viewId) is TextView tv)
        {
            tv.Text = title;
            tv.Set200x44Size(res);
        }
    }

    public static void SetupTextField(this View view, int viewId, Resources res)
    {
        var tv = view.FindViewById<EditText>(viewId);
        tv.Set200x44Size(res);
    }

    // ReSharper disable once InconsistentNaming
    private static void Set200x44Size(this View v, Resources res)
    {
        if (v.LayoutParameters is ViewGroup.LayoutParams lp)
        {
            lp.Width = Convert.ToInt32(TypedValue.ApplyDimension(ComplexUnitType.Dip, CoreResource.TitleWidth, res.DisplayMetrics));
            lp.Height = Convert.ToInt32(TypedValue.ApplyDimension(ComplexUnitType.Dip, CoreResource._44px, res.DisplayMetrics));
        }
    }
}