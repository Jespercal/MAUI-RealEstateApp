using Microsoft.Maui.ApplicationModel.Communication;
using RealEstateApp.Models;
using RealEstateApp.Services;
using RealEstateApp.Views;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Input;

namespace RealEstateApp.ViewModels;

[QueryProperty(nameof(PropertyListItem), "MyPropertyListItem")]
public class PropertyDetailPageViewModel : BaseViewModel
{
    private readonly IPropertyService service;
    private readonly PreferenceService _ps;
    public PropertyDetailPageViewModel(IPropertyService service, PreferenceService ps)
    {
        this.service = service;
        _ps = ps;
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
        if((string)e == "play") {
            IsPlaying = true;
            ttsCancelToken = new CancellationTokenSource();
            await TextToSpeech.Default.SpeakAsync(Property.Description, new SpeechOptions()
            {
                Locale = _ps.TTS_Locale,
                Pitch = (float)_ps.TTS_Pitch,
                Volume = (float)_ps.TTS_Volume
            }, cancelToken: ttsCancelToken.Token);
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
    public ICommand ShowLocationCommand => showLocationCommand ??= new Command(async ( type ) => await ExecuteShowLocation((string)type));
    async Task ExecuteShowLocation( string type )
    {
        TryStopTTS();

        await Map.TryOpenAsync( Property.Latitude.Value, Property.Longitude.Value, new MapLaunchOptions()
        {
            NavigationMode = (type == "navigate" ? NavigationMode.Driving : NavigationMode.None)
        });
    }
    
    private Command pageDisappearingCommand;
    public ICommand PageDisappearingCommand => pageDisappearingCommand ??= new Command(() =>
    {
        TryStopTTS();
    });
    
    private Command _goToImageListCommand;
    public ICommand GoToImageListCommand => _goToImageListCommand ??= new Command(async () =>
    {
        await Shell.Current.GoToAsync(nameof(ImageListPage), new Dictionary<string, object>
        {
            {"Property", Property }
        });
    });

    private Command _callPhoneCommand;
    public ICommand CallPhoneCommand => _callPhoneCommand ??= new Command(() =>
    {
        PhoneDialer.Default.Open(Property.Vendor.Phone);
    });
    

    private Command _sendEmailCommand;
    public ICommand SendEmailCommand => _sendEmailCommand ??= new Command(async () =>
    {
        var attachmentFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, "property.txt");
        await File.WriteAllTextAsync(attachmentFilePath, $"{Property.Address}");

        var email = new EmailMessage($"Hello, {Property.Vendor.FirstName}, regarding the property {Property.Address}", "About the property");
        email.Attachments.Add(new EmailAttachment(attachmentFilePath));
        await Email.Default.ComposeAsync(email);
    });


    private Command _goToWebsiteCommand;
    public ICommand GoToWebsiteCommand => _goToWebsiteCommand ??= new Command(async () =>
    {
        await Browser.Default.OpenAsync(Property.NeighbourhoodUrl, BrowserLaunchMode.SystemPreferred);
    });
    
    private Command _openContractCommand;
    public ICommand OpenContractCommand => _openContractCommand ??= new Command(async () =>
    {
        await Launcher.Default.OpenAsync(new OpenFileRequest("Show Contract", new ReadOnlyFile(Property.ContractFilePath)));
    });

    private Command _openFBCommand;
    public ICommand OpenFBCommand => _openFBCommand ??= new Command(async () =>
    {
        await Launcher.Default.TryOpenAsync("fb://");
    });
    
    private Command _sharePropertyCommand;
    public ICommand SharePropertyCommand => _sharePropertyCommand ??= new Command(async (type) =>
    {
        string shareType = (string)type;
        switch(shareType)
        {
            case "text":
                await Share.Default.RequestAsync(new ShareTextRequest()
                {
                    Uri = Property.NeighbourhoodUrl,
                    Subject = "A property you may be interested in",
                    Text = $"Located at {Property.Address}, priced at {Property.Price}, with {Property.Beds} bed(s), {Property.Baths} bath(s) and {Property.Parking} parking(s).",
                    Title = "Share Property"
                });
                break;
            case "file":
                await Share.Default.RequestAsync(new ShareFileRequest()
                {
                    File = new ShareFile(Property.ContractFilePath),
                    Title = "Share Property Contract"
                });
                break;
            case "clipboard":
                var serialized = JsonSerializer.Serialize<Property>(Property, new JsonSerializerOptions()
                {
                    MaxDepth= 3,
                    ReferenceHandler = ReferenceHandler.Preserve
                });
                await Clipboard.Default.SetTextAsync(serialized);
                break;
        }
    });
}
