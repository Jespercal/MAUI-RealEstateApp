using RealEstateApp.Models;
using RealEstateApp.Services;
using RealEstateApp.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using ZXing.Net.Maui;

namespace RealEstateApp.ViewModels;

public class BarcodeScannerViewModel : BaseViewModel, ILeaving
{
	
    public BarcodeScannerViewModel()
    {
    }

    public void OnLeaving()
    {

    }

    private Command _barcodeScannedCommand;
    public ICommand BarcodeScannedCommand => _barcodeScannedCommand ??= new Command(async (r) =>
    {
        BarcodeResult[] results = (BarcodeResult[])r;
        if(results.Length > 0)
        {
            await Browser.OpenAsync(results[0].Value);
        }

        await Shell.Current.GoToAsync("//propertylist");
    });
}
