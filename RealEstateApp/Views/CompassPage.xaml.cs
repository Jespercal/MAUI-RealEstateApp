using RealEstateApp.ViewModels;

namespace RealEstateApp.Views;

public partial class CompassPage : ContentPage
{
	public CompassPage( CompassViewModel vm )
	{
		InitializeComponent();
		BindingContext = vm;
	}
}