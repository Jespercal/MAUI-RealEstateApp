using RealEstateApp.Models;
using RealEstateApp.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RealEstateApp.ViewModels;

[QueryProperty(nameof(Mode), "mode")]
[QueryProperty(nameof(Property), "MyProperty")]
public class AddEditPropertyPageViewModel : BaseViewModel
{
    readonly IPropertyService service;

    public AddEditPropertyPageViewModel(IPropertyService service)
    {
        this.service = service;
        Agents = new ObservableCollection<Agent>(service.GetAgents());
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
           StatusMessage = "Please fill in all required fields";
            StatusColor = Colors.Red;
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
    public ICommand CancelSaveCommand => cancelSaveCommand ??= new Command(async () => await Shell.Current.GoToAsync(".."));
    
    private Command copyCurrentLocationCommand;
    public ICommand CopyCurrentLocationCommand => copyCurrentLocationCommand ??= new Command(async () =>
    {
        IsBusy = true;

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
            await Shell.Current.DisplayAlert("Error", "Could not get your current location...", "OK");
        }

        IsBusy = false;
    });
    
    private Command locationFromAddressCommand;
    public ICommand LocationFromAddressCommand => locationFromAddressCommand ??= new Command(async () =>
    {
        IsBusy = true;

        try
        {
            Location loc = (await Geocoding.GetLocationsAsync(Property.Address)).First();
            Property.Latitude = loc.Latitude;
            Property.Longitude = loc.Longitude;

            OnPropertyChanged("Property");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "Could not get location from address...", "OK");
        }

        IsBusy = false;
    });
}
