using PaulN10V.MvxTabbedNavigation.Demo.Core;
using PaulN10V.MvxTabbedNavigation.Demo.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using UIKit;

// ReSharper disable once CheckNamespace
namespace PaulN10V.MvxTabbedNavigation.Demo.Platforms.iOS;

internal static class ViewEx
{
    public static void BindBackground<TOwningTarget, TSource>(this MvxFluentBindingDescriptionSet<TOwningTarget, TSource> set, UIView view)
        where TOwningTarget : class, IMvxBindingContextOwner
        where TSource : IHasColor 
        => set.Bind(view).For(v => v.BackgroundColor).To(vm => vm.Color).WithConversion("NativeColor");


    public static void Setup(this UIStackView stackView, UILayoutConstraintAxis axis)
    {
        stackView.TranslatesAutoresizingMaskIntoConstraints = false;
        stackView.Axis = axis;
        stackView.Distribution = UIStackViewDistribution.EqualSpacing;
        stackView.Alignment = UIStackViewAlignment.Center;
        stackView.Spacing = 8;
    }

    public static UIButton CreateTitledButton(string title)
    {
        var btn = new UIButton { TranslatesAutoresizingMaskIntoConstraints = false, TintColor = UIColor.White };
        btn.SetTitle(title, UIControlState.Normal);

        btn.Set200x44Size();
        return btn;
    }

    public static UIStackView CreateStackView(UILayoutConstraintAxis axis, params UIView[] views)
    {
        var stateStack = new UIStackView(views);
        stateStack.Setup(axis);
        return stateStack;
    }

    public static void CenterOf(this UIView stackView, UIView viewToCenterOf)
    {
        stackView.CenterXAnchor.ConstraintEqualTo(viewToCenterOf.CenterXAnchor).Active = true;
        stackView.CenterYAnchor.ConstraintEqualTo(viewToCenterOf.CenterYAnchor).Active = true;
    }

    public static UILabel CreateTitledLabel(string title)
    {
        var lbl = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false, TextColor = UIColor.White, TextAlignment = UITextAlignment.Center, Text = title };
        lbl.Set200x44Size();
        return lbl;
    }

    public static UITextField CreateTextField()
    {
        var txt = new UITextField { TranslatesAutoresizingMaskIntoConstraints = false, BackgroundColor = UIColor.White };
        txt.Set200x44Size();
        return txt;
    }

    // ReSharper disable once InconsistentNaming
    private static void Set200x44Size(this UIView view)
    {
        view.WidthAnchor.ConstraintEqualTo(Resource.TitleWidth).Active = true;
        view.HeightAnchor.ConstraintEqualTo(Resource._44px).Active = true;
    }
}