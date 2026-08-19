using MvvmCross.Platforms.Ios.Presenters;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.Presenters;
using MvvmCross.Presenters.Attributes;
using MvvmCross.Presenters.Hints;
using MvvmCross.ViewModels;
using PaulN10V.MvxTabbedNavigation.Presenters.Attributes;
using PaulN10V.MvxTabbedNavigation.Presenters.Hints;

namespace PaulN10V.MvxTabbedNavigation.Platforms.iOS.Presenters;

public class TabbedViewPresenter : MvxIosViewPresenter
{
    // ReSharper disable once ConvertToPrimaryConstructor
    public TabbedViewPresenter(UIWindow window) : base(window) { }

    public override void RegisterAttributeTypes()
    {
        base.RegisterAttributeTypes();

        AttributeTypesToActionsDictionary.Register<OverTopPresentationAttribute>(
            (_, attribute, request) =>
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                var viewController = this.CreateViewControllerFor(request) as UIViewController;
                return ShowOverTopViewController(viewController!, attribute, request);
            },
            CloseOverTopViewController);
    }

    public override Task<bool> ChangePresentation(MvxPresentationHint hint)
    {
        return hint switch
        {
            ClearStackPresentationHint when ClearStack() =>
                Task.FromResult(true),
            MvxPopToRootPresentationHint popToRootPresentationHint when PopToRoot(popToRootPresentationHint) =>
                Task.FromResult(true),
            _ => base.ChangePresentation(hint)
        };
    }

    private bool ClearStack()
    {
        if (MasterNavigationController?.TopViewController is UITabBarController { SelectedViewController: UINavigationController currentNavController })
        {
            currentNavController.ViewControllers = [];
            return true;
        }
        else if (MasterNavigationController?.TopViewController is UINavigationController navController)
        {
            navController.ViewControllers = [];
            return true;
        }

        return false;
    }

    private bool PopToRoot(MvxPopToRootPresentationHint popToRootPresentationHint)
    {
        if (MasterNavigationController?.TopViewController is UITabBarController { SelectedViewController: UINavigationController currentNavController })
        {
            currentNavController.PopToRootViewController(popToRootPresentationHint.Animated);
            return true;
        }
        else if (MasterNavigationController != null)
        {
            MasterNavigationController.PopToRootViewController(popToRootPresentationHint.Animated);
            return true;
        }

        return false;
    }

    protected virtual Task<bool> ShowOverTopViewController(
        UIViewController viewController,
        OverTopPresentationAttribute attribute,
        MvxViewModelRequest request)
    {
        ValidateArguments(viewController, attribute);

        if (MasterNavigationController != null)
        {
            PushViewControllerIntoStack(MasterNavigationController, viewController, ConvertToChildPresentationAttribute(attribute));
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    protected virtual Task<bool> CloseOverTopViewController(IMvxViewModel viewModel, OverTopPresentationAttribute attribute)
    {
        ValidateArguments(viewModel, attribute);

        // if the current root is a NavigationController, close it in the stack
        if (MasterNavigationController != null && TryCloseViewControllerInsideStack(MasterNavigationController, viewModel, ConvertToChildPresentationAttribute(attribute)))
            return Task.FromResult(true);

        return Task.FromResult(false);
    }

    private MvxChildPresentationAttribute ConvertToChildPresentationAttribute(OverTopPresentationAttribute attribute)
        => new MvxChildPresentationAttribute { ViewModelType = attribute.ViewModelType, ViewType = attribute.ViewType };

    private static void ValidateArguments(UIViewController viewController, MvxBasePresentationAttribute attribute)
    {
        if (viewController == null)
            throw new ArgumentNullException(nameof(viewController));

        if (attribute == null)
            throw new ArgumentNullException(nameof(attribute));
    }

    private static void ValidateArguments(IMvxViewModel viewModel, MvxBasePresentationAttribute attribute)
    {
        if (viewModel == null)
            throw new ArgumentNullException(nameof(viewModel));

        if (attribute == null)
            throw new ArgumentNullException(nameof(attribute));
    }

    protected override Task<bool> ShowChildViewController(UIViewController viewController, MvxChildPresentationAttribute attribute, MvxViewModelRequest request)
    {
        if (MasterNavigationController?.ViewControllers?.Length >= 2)
        {
            PushViewControllerIntoStack(MasterNavigationController, viewController, attribute);
            return Task.FromResult(true);
        }

        return base.ShowChildViewController(viewController, attribute, request);
    }
}