using RealEstateApp.ViewModels;

namespace RealEstateApp.Views;

public partial class PropertyDetailPage : ContentPage
{
    private readonly PropertyDetailPageViewModel _vm;
    public PropertyDetailPage(PropertyDetailPageViewModel vm)
	{
		InitializeComponent();
        _vm = vm;
		BindingContext = _vm;

        Disappearing += PropertyDetailPage_Disappearing;
	}

    private void PropertyDetailPage_Disappearing(object sender, EventArgs e)
    {
        _vm.PageDisappearingCommand
    }
}