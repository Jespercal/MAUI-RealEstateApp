using RealEstateApp.Models;
using RealEstateApp.Services;
using RealEstateApp.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace RealEstateApp.ViewModels;

[QueryProperty(nameof(Property), "Property")]
public class SecureViewModel : BaseViewModel
{
    private readonly SettingsService _ss;
    public SecureViewModel( SettingsService ss )
    {
        _ss = ss;
    }

    private Command _logoutCommand;
    public ICommand LogoutCommand => _logoutCommand ??= new Command(async () =>
    {
        await _ss.LogoutAsync();
        await Shell.Current.GoToAsync("//propertylist");
    });
}
