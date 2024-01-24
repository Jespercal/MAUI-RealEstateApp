using RealEstateApp.Models;
using RealEstateApp.Services;
using RealEstateApp.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RealEstateApp.ViewModels;

[QueryProperty(nameof(Mode), "mode")]
[QueryProperty(nameof(Property), "MyProperty")]
public class AddEditPropertyPageViewModel : BaseViewModel, IDisposable
{
    readonly IPropertyService service;
    readonly ConnectivityService _connectivityService;
    readonly BatteryService batteryService;

    private string _alertMessage = null;
    public string AlertMessage
    {
        get { return _alertMessage; }
        set { _alertMessage = value;OnPropertyChanged(); }
    }
    
    private Color _alertColor = Colors.PaleVioletRed;
    public Color AlertColor
    {
        get { return _alertColor; }
        set { _alertColor = value;OnPropertyChanged(); }
    }

    private Color _alertTextColor = Colors.White;
    public Color AlertTextColor
    {
        get { return _alertTextColor; }
        set { _alertTextColor = value; OnPropertyChanged(); }
    }


    public AddEditPropertyPageViewModel(IPropertyService service, ConnectivityService connectivityService, BatteryService battery)
    {
        this.batteryService = battery;
        this.service = service;
        this._connectivityService = connectivityService;
        Agents = new ObservableCollection<Agent>(service.GetAgents());

        _connectivityService.OnStatusChanged += Connectivity_ConnectivityChanged;
        if(!_connectivityService.IsConnected)
        {
            AlertColor = Colors.PaleVioletRed;
            AlertTextColor = Colors.White;
            AlertMessage = "You need internet to use location";
        }
        batteryService.OnStatusChanged += Battery_StatusChanged;
    }

    public void Dispose()
    {
        _connectivityService.OnStatusChanged -= Connectivity_ConnectivityChanged;
        batteryService.OnStatusChanged -= Battery_StatusChanged;
    }

    private async void Battery_StatusChanged()
    {
        if (batteryService.ChargeLevel <= 0.2)
        {
            if (batteryService.EnergySaverStatus == EnergySaverStatus.On)
            {
                AlertMessage = "Energy-saving is on";
                AlertColor = Colors.Green;
                AlertTextColor = Colors.White;
            }
            else if (batteryService.State == BatteryState.NotCharging)
            {
                AlertMessage = "Battery is running low!";
                AlertColor = Colors.Red;
                AlertTextColor = Colors.White;
            }
            else if (batteryService.State == BatteryState.Charging)
            {
                AlertMessage = "Battery low but charging";
                AlertColor = Colors.Yellow;
                AlertTextColor = Colors.Black;
            }
        }
        else
        {
            AlertMessage = null;
        }
    }
    private async void Connectivity_ConnectivityChanged(bool status)
    {
        AlertColor = !status ? Colors.PaleVioletRed : Colors.Green;
        AlertMessage = !status ? "Internet connect lost!" : "Internet connection regained!";
    }

    public string Mode { get; set; }

    #region PROPERTIES
    public ObservableCollection<Agent> Agents { get; }

    private Property _property;
    public Property Property
    {
        get => _property;
        set
        {
            SetProperty(ref _property, value);
            Title = Mode == "newproperty" ? "Add Property" : "Edit Property";

            if (_property.AgentId != null)
            {
                SelectedAgent = Agents.FirstOrDefault(x => x.Id == _property?.AgentId);
            }
        }
    }

    private Agent _selectedAgent;
    public Agent SelectedAgent
    {
        get => _selectedAgent;
        set
        {
            if (Property != null)
            {
                _selectedAgent = value;
                Property.AgentId = _selectedAgent?.Id;
            }
        }
    }

    string statusMessage;
    public string StatusMessage
    {
        get { return statusMessage; }
        set { SetProperty(ref statusMessage, value); }
    }

    Color statusColor;
    public Color StatusColor
    {
        get { return statusColor; }
        set { SetProperty(ref statusColor, value); }
    }
    #endregion


    private Command savePropertyCommand;
    public ICommand SavePropertyCommand => savePropertyCommand ??= new Command(async () => await SaveProperty());
    private async Task SaveProperty()
    {
        if (IsValid() == false)
        {
            if(await Permissions.CheckStatusAsync<Permissions.Vibrate>() == PermissionStatus.Granted)
                Vibration.Vibrate(5000);

           StatusMessage = "Please fill in all required fields";
            StatusColor = Colors.Red;

            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
        }
        else
        {
            service.SaveProperty(Property);
            await Shell.Current.GoToAsync("///propertylist");
        }
    }

    public bool IsValid()
    {
        if (string.IsNullOrEmpty(Property.Address)
            || Property.Beds == null
            || Property.Price == null
            || Property.AgentId == null)
            return false;
        return true;
    }

    private Command cancelSaveCommand;
    public ICommand CancelSaveCommand => cancelSaveCommand ??= new Command(async () =>
    {
        if (await Permissions.CheckStatusAsync<Permissions.Vibrate>() == PermissionStatus.Granted)
            Vibration.Cancel();

        await Shell.Current.GoToAsync("..");
    });
    
    private Command copyCurrentLocationCommand;
    public ICommand CopyCurrentLocationCommand => copyCurrentLocationCommand ??= new Command(async () =>
    {
        IsBusy = true;

        HapticFeedback.Default.Perform(HapticFeedbackType.Click);

        try
        {
            Location loc = await Geolocation.GetLocationAsync();
            Property.Latitude = loc.Latitude;
            Property.Longitude = loc.Longitude;
            var rawAddress = (await Geocoding.GetPlacemarksAsync(loc.Latitude, loc.Longitude)).First();
            Property.Address = string.Format("{0} {1}, {2} {3}", rawAddress.Thoroughfare, rawAddress.SubThoroughfare, rawAddress.PostalCode, rawAddress.CountryName);

            OnPropertyChanged("Property");
        }
        catch (Exception ex)
        {
            AlertColor = Colors.PaleVioletRed;
            AlertMessage = "Could not get your current location...";
        }

        IsBusy = false;
    });
    
    private Command locationFromAddressCommand;
    public ICommand LocationFromAddressCommand => locationFromAddressCommand ??= new Command(async () =>
    {
        IsBusy = true;

        HapticFeedback.Default.Perform(HapticFeedbackType.Click);

        try
        {
            Location loc = (await Geocoding.GetLocationsAsync(Property.Address)).First();
            Property.Latitude = loc.Latitude;
            Property.Longitude = loc.Longitude;

            OnPropertyChanged("Property");
        }
        catch (Exception ex)
        {
            AlertColor = Colors.PaleVioletRed;
            AlertMessage = "Could not get location from address...";
        }

        IsBusy = false;
    });
    
    private Command closeAlertCommand;
    public ICommand CloseAlertCommand => closeAlertCommand ??= new Command(async () =>
    {
        AlertMessage = null;
    });

    private bool _flashlightOn;

    public bool FlashlightOn
    {
        get { return _flashlightOn; }
        set { _flashlightOn = value; OnPropertyChanged(); }
    }

    private Color flashlightColor = Colors.Black;

    public Color FlashlightColor
    {
        get { return flashlightColor; }
        set { flashlightColor = value; }
    }



    private Command toggleFlashlightCommand;
    public ICommand ToggleFlashlightCommand => toggleFlashlightCommand ??= new Command(async () =>
    {
        if (FlashlightOn)
        {
            await Flashlight.Default.TurnOffAsync();
            FlashlightColor = Colors.Black;
            FlashlightOn = false;
        }
        else
        {
            await Flashlight.Default.TurnOnAsync();
            FlashlightColor = Colors.White;
            FlashlightOn = true;
        }
    });

    private Command goToCompassCommand;
    public ICommand GoToCompassCommand => goToCompassCommand ??= new Command(async () =>
    {
        await Shell.Current.GoToAsync(nameof(CompassPage), new Dictionary<string,object>()
        {
            { "property", Property }
        });
        
        // Wait until we return to this page again
        while(Shell.Current.CurrentPage.GetType() != typeof(AddEditPropertyPage))
        {
            await Task.Delay(10);
        }

        // We are back, now we update the Property-property
        OnPropertyChanged(nameof(Property));
    });
}
