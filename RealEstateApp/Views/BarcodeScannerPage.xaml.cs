using RealEstateApp.ViewModels;
using ZXing.Net.Maui;

namespace RealEstateApp.Views;

public partial class BarcodeScannerPage : ContentPage
{
    private readonly BarcodeScannerViewModel viewmodel;

    public BarcodeScannerPage( BarcodeScannerViewModel vm )
	{
		InitializeComponent();
        viewmodel = vm;
        BindingContext = viewmodel;

        cameraBarcodeReaderView.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.All,
            AutoRotate = true,
            Multiple = true
        };
    }

    public bool StopScanning { get; set; }
    private void cameraBarcodeReaderView_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {
        if (StopScanning)
            return;

        StopScanning = true;
        viewmodel.BarcodeScannedCommand.Execute(e.Results);
    }

    private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        StopScanning = false;
    }
}