using Microsoft.Extensions.Logging;
using RealEstateApp.Helpers;
using RealEstateApp.Repositories;
using RealEstateApp.Services;
using RealEstateApp.ViewModels;
using RealEstateApp.Views;
using ZXing.Net.Maui.Controls;

namespace RealEstateApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("fa-solid-900.ttf", "FA-solid");
            })
            .UseBarcodeReader()
            .UseMauiMaps();

        builder.Services.AddSingleton<IPropertyService, MockRepository>();
        builder.Services.AddSingleton<ConnectivityService>();
        builder.Services.AddSingleton<BatteryService>();

        builder.Services.AddSingleton<PropertyListPage>();
        builder.Services.AddSingleton<PropertyListPageViewModel>();

        builder.Services.AddTransient<PropertyDetailPage>();
        builder.Services.AddTransient<PropertyDetailPageViewModel>();

        builder.Services.AddPage<AddEditPropertyPage>();
        builder.Services.AddTransient<AddEditPropertyPageViewModel>();

        builder.Services.AddPage<CompassPage>();
        builder.Services.AddTransient<CompassViewModel>();

        builder.Services.AddPage<HeightCalculatorPage>();
        builder.Services.AddTransient<HeightCalculatorPageViewModel>();

        builder.Services.AddPage<ImageListPage>();
        builder.Services.AddTransient<ImageListViewModel>();
        
        builder.Services.AddPage<BarcodeScannerPage>();
        builder.Services.AddTransient<BarcodeScannerViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        MauiApp app = builder.Build();

        app.Services.GetService<BatteryService>();
        app.Services.GetService<ConnectivityService>();

        return app;
    }
}
