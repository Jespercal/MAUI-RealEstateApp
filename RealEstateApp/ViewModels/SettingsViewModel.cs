using RealEstateApp.Models;
using RealEstateApp.Services;
using RealEstateApp.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using ZXing.Net.Maui;

namespace RealEstateApp.ViewModels;

public class SettingsViewModel : BaseViewModel, ILeaving
{
    private double _tts_pitch;
    public double TTS_Pitch { get => _tts_pitch; set { _tts_pitch = value; OnPropertyChanged(); } }

    private double _tts_volume;
    public double TTS_Volume { get => _tts_volume; set { _tts_volume = value; OnPropertyChanged(); } }

    private readonly PreferenceService _ps;
    public SettingsViewModel( PreferenceService ps)
    {
        _ps = ps;
        TTS_Pitch = _ps.TTS_Pitch;
        TTS_Volume = _ps.TTS_Volume;
    }

    public void OnLeaving()
    {
        _ps.TTS_Pitch = TTS_Pitch;
        _ps.TTS_Volume = TTS_Volume;

        _ps.Save();
    }
}
