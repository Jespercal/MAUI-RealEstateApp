using RealEstateApp.Models;
using RealEstateApp.Services;
using RealEstateApp.Views;
using System.Diagnostics;
using System.Windows.Input;

namespace RealEstateApp.ViewModels;

[QueryProperty(nameof(PropertyListItem), "MyPropertyListItem")]
public class PropertyDetailPageViewModel : BaseViewModel
{
    private readonly IPropertyService service;
    public PropertyDetailPageViewModel(IPropertyService service)
    {
        this.service = service;
    }

    Property property;
    public Property Property { get => property; set { SetProperty(ref property, value); } }


    Agent agent;
    public Agent Agent { get => agent; set { SetProperty(ref agent, value); } }

    private bool _isPlaying;

    public bool IsPlaying
    {
        get { return _isPlaying; }
        set { _isPlaying = value;OnPropertyChanged(); }
    }

    private CancellationTokenSource ttsCancelToken;
    private Command tTSCommand;
    public ICommand TTSCommand => tTSCommand ??= new Command( async (e) =>
    {
        Debug.WriteLine(Shell.Current.GetType().Name);
        Debug.WriteLine(Shell.Current.CurrentPage.GetType().Name);
        if((string)e == "play") {
            IsPlaying = true;
            ttsCancelToken = new CancellationTokenSource();
            await TextToSpeech.Default.SpeakAsync(Property.Description, cancelToken: ttsCancelToken.Token);
            IsPlaying = false;
        } else if((string)e == "stop") {
            TryStopTTS();
        }
    });

    public void TryStopTTS()
    {
        if (ttsCancelToken?.IsCancellationRequested ?? true)
            return;

        ttsCancelToken.Cancel();
        IsPlaying = false;
    }


    PropertyListItem propertyListItem;
    public PropertyListItem PropertyListItem
    {
        set
        {
            SetProperty(ref propertyListItem, value);
           
            Property = propertyListItem.Property;
            Agent = service.GetAgents().FirstOrDefault(x => x.Id == Property.AgentId);
        }
    }

    private Command editPropertyCommand;
    public ICommand EditPropertyCommand => editPropertyCommand ??= new Command(async () => await GotoEditProperty());
    async Task GotoEditProperty()
    {
        TryStopTTS();
        await Shell.Current.GoToAsync($"{nameof(AddEditPropertyPage)}?mode=editproperty", true, new Dictionary<string, object>
        {
            {"MyProperty", Property }
        });
    }
    
    private Command showLocationCommand;
    public ICommand ShowLocationCommand => showLocationCommand ??= new Command(async () => await ExecuteShowLocation());
    async Task ExecuteShowLocation()
    {
        TryStopTTS();
        await Map.TryOpenAsync( Property.Latitude.Value, Property.Longitude.Value );
    }
    
    private Command pageDisappearingCommand;
    public ICommand PageDisappearingCommand => pageDisappearingCommand ??= new Command(() =>
    {
        TryStopTTS();
    });
}
