using RealEstateApp.ViewModels;

namespace RealEstateApp.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage( LoginViewModel vm )
	{
		InitializeComponent();
		BindingContext = vm;
		NavigatedTo += (e, t) =>
		{
			vm.OnOpeningCommand.Execute(null);
		};
	}
}
