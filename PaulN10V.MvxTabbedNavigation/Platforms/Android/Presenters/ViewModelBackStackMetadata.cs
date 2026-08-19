using MvvmCross.ViewModels;
using PaulN10V.MvxTabbedNavigation.Platforms.Android.Presenters.Attributes;

namespace PaulN10V.MvxTabbedNavigation.Platforms.Android.Presenters;

public class ViewModelBackStackMetadata
{
    public bool IsPending { get; private set; }

    public string TabId { get; private set; } = null!;

    private TabPresentationAttribute? _pendingPresentationAttribute;

    private MvxViewModelRequest? _pendingVmRequest;

    private Stack<Type> ViewModelTypesStack { get; } = new ();

    private ViewModelBackStackMetadata(TabPresentationAttribute attribute, MvxViewModelRequest vmRequest)
    {
        if(attribute == null)
            throw new ArgumentNullException(nameof(attribute));

        if (attribute.ViewModelType == null)
            throw new ArgumentException($"{nameof(attribute)}.{nameof(attribute.ViewModelType)} is null");

        IsPending = true;
        _pendingPresentationAttribute = attribute;
        _pendingVmRequest = vmRequest ?? throw new ArgumentNullException(nameof(vmRequest));
        Initialize(attribute.TabId, vmRequest.ViewModelType!);
    }

    private ViewModelBackStackMetadata(string tabId, Type viewModelType)
    {
        if (tabId == null)
            throw new ArgumentNullException(nameof(tabId));

        if (viewModelType == null)
            throw new ArgumentNullException(nameof(viewModelType));

        IsPending = false;
        Initialize(tabId, viewModelType);
    }

    private void Initialize(string tabId, Type viewModelType)
    {
        TabId = tabId;
        ViewModelTypesStack.Push(viewModelType);
    }

    public (TabPresentationAttribute attribute, MvxViewModelRequest vmRequest) ExtractPendingRequestData()
    {
        IsPending = false;
        var result = (_pendingPresentationAttribute!, _pendingVmRequest!);
        _pendingPresentationAttribute = null;
        _pendingVmRequest = null;
        return result;
    }

    public static ViewModelBackStackMetadata CreateFromPendingRequest(TabPresentationAttribute attribute, MvxViewModelRequest vmRequest) 
        => new ViewModelBackStackMetadata(attribute, vmRequest);

    public static ViewModelBackStackMetadata Create(string tabId, Type viewModelType) 
        => new ViewModelBackStackMetadata(tabId, viewModelType);

    public bool HasViewModelInStack(Type viewModel) => ViewModelTypesStack.Contains(viewModel);

    public void Push(Type viewModelType) => ViewModelTypesStack.Push(viewModelType);

    public void Pop() => ViewModelTypesStack.Pop();

    public void PopIfNeeded(int count)
    {
        if (count == ViewModelTypesStack.Count - 1)
            ViewModelTypesStack.Pop();
    }

    public void Clear() => ViewModelTypesStack.Clear();

    public void PopToRoot()
    {
        if (ViewModelTypesStack.Count > 0)
        {
            var firstVmType = ViewModelTypesStack.ElementAt(0);
            ViewModelTypesStack.Clear();
            ViewModelTypesStack.Push(firstVmType); 
        }
    }
}