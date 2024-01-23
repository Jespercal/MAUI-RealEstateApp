using RealEstateApp.Models;
using RealEstateApp.Services;
using RealEstateApp.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace RealEstateApp.ViewModels;
public class PropertyListPageViewModel : BaseViewModel
{
    public ObservableCollection<PropertyListItem> PropertiesCollection { get; } = new();

    private readonly IPropertyService service;

    public PropertyListPageViewModel(IPropertyService service)
    {
        Title = "Property List";
        this.service = service;
    }

    bool isRefreshing;
    public bool IsRefreshing
    {
        get => isRefreshing;
        set => SetProperty(ref isRefreshing, value);
    }

    private Command getPropertiesCommand;
    public ICommand GetPropertiesCommand => getPropertiesCommand ??= new Command(async () => await GetPropertiesAsync());
    
    private Command sortPropertiesCommand;
    public ICommand SortPropertiesCommand => sortPropertiesCommand ??= new Command(async () => await SortAsync());

    async Task GetPropertiesAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if(currentLocation == null)
            {
                currentLocation = await Geolocation.GetLastKnownLocationAsync();
                if (currentLocation == null)
                    currentLocation = await Geolocation.GetLocationAsync();
            }

            List<Property> properties = service.GetProperties();
            List<PropertyListItem> propertiesList = new List<PropertyListItem>();

            foreach (Property property in properties)
            {
                var item = new PropertyListItem(property);
                item.CalcDistance(currentLocation);
                propertiesList.Add(item);
            }

            if (PropertiesCollection.Count != 0)
                PropertiesCollection.Clear();

            foreach (PropertyListItem listitem in propertiesList)
                PropertiesCollection.Add(listitem);

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to get monkeys: {ex.Message}");
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    private Location currentLocation;

    public async Task SortAsync()
    {
        var sorted = PropertiesCollection.ToList().OrderBy(e => e.Distance ?? 0).ToList();

        if (PropertiesCollection.Count != 0)
            PropertiesCollection.Clear();

        foreach (PropertyListItem listitem in sorted)
            PropertiesCollection.Add(listitem);
    }

    private Command goToDetailsCommand;
    public ICommand GoToDetailsCommand => goToDetailsCommand ??= new Command(async (e) => await GoToDetails((PropertyListItem)e));
    async Task GoToDetails(PropertyListItem propertyListItem)
    {
        if (propertyListItem == null)
            return;

        await Shell.Current.GoToAsync(nameof(PropertyDetailPage), true, new Dictionary<string, object>
        {
            {"MyPropertyListItem", propertyListItem }
        });
    }

    private Command goToAddPropertyCommand;
    public ICommand GoToAddPropertyCommand => goToAddPropertyCommand ??= new Command(async () => await GotoAddProperty());
    async Task GotoAddProperty()
    {
        await Shell.Current.GoToAsync($"{nameof(AddEditPropertyPage)}?mode=newproperty", true, new Dictionary<string, object>
        {
            {"MyProperty", new Property() }
        });
    }
}
