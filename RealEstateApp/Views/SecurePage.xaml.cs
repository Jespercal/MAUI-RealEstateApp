using RealEstateApp.ViewModels;

namespace RealEstateApp.Views;

public partial class SecurePage : ContentPage
{
	public SecurePage( SecureViewModel vm )
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
