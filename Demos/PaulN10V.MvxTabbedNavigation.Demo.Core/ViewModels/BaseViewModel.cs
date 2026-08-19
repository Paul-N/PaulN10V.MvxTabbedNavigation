using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Presenters.Hints;
using MvvmCross.ViewModels;
using System.Drawing;
using System.Windows.Input;
// ReSharper disable ConvertToPrimaryConstructor
// ReSharper disable MemberCanBePrivate.Global

namespace PaulN10V.MvxTabbedNavigation.Demo.Core.ViewModels;

public class BaseViewModel : MvxViewModel, IHasColor
{
	private readonly IMvxNavigationService _navigationService;
		
	private int _value;

	public int Value
	{
		get => _value;
		set => SetProperty(ref _value, value);
	}

	public ICommand IncrCommand => new MvxCommand(() => Value++);

	public ICommand DecrCommand => new MvxCommand(() => Value--);

	public ICommand OpenNewCommand => new MvxCommand(() => _navigationService.Navigate<NewScreenViewModel>());

	public ICommand OpenOverTopCommand => new MvxCommand(() => _navigationService.Navigate<OverTopViewModel>());

	public ICommand PopToRootCommand => new MvxCommand(() => _navigationService.ChangePresentation(new MvxPopToRootPresentationHint()));

	public ICommand CloseSelfCommand => new MvxCommand(() => _navigationService.Close(this));

	public Color Color { get; private set; }

	protected BaseViewModel(IMvxNavigationService navigationService)
	{
		_navigationService = navigationService;
		Color = Resource.GetRandomColor();
	}
}