using RealEstateApp.ViewModels;

namespace RealEstateApp.Views;

public partial class ImageListPage : ContentPage
{
	public ImageListPage( ImageListViewModel vm )
	{
		InitializeComponent();
		BindingContext = vm;
	}
}