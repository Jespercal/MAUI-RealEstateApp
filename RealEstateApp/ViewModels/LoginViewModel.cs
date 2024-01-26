using RealEstateApp.Models;
using RealEstateApp.Services;
using RealEstateApp.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace RealEstateApp.ViewModels;

[QueryProperty(nameof(Property), "Property")]
public class LoginViewModel : BaseViewModel
{
    private string username;
    public string Username { get => username; set { username = value; OnPropertyChanged(); } }

    private string password;
    public string Password { get => password; set { password = value; OnPropertyChanged(); } }

    private string message;
    public string Message { get => message; set { message = value; OnPropertyChanged(); } }

    private SettingsService _ss;
    public LoginViewModel( SettingsService ss )
    {
        _ss = ss;
    }

    private Command loginCommand;
    public ICommand LoginCommand => loginCommand ??= new Command(async () =>
    {
        int result = await _ss.TryLoginAsync((username ?? "").ToLower(), (password ?? ""));
        if(result == 1)
        {
            await Shell.Current.GoToAsync(nameof(SecurePage));
        }

        Message = "Username or password incorrect!";
    });

    private Command onOpeningCommand;
    public ICommand OnOpeningCommand => onOpeningCommand ??= new Command(async () =>
    {
        if(_ss.LoginDetails.Succeeded)
        {
            await Shell.Current.GoToAsync(nameof(SecurePage));
        }
    });
}
