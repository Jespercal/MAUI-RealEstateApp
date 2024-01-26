using RealEstateApp.Models;
using RealEstateApp.Services;
using RealEstateApp.Views;
using System.Collections.Generic;
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

    private Locale _tts_locale;
    public Locale TTS_Locale { get => _tts_locale; set { _tts_locale = value; OnPropertyChanged(); } }

    public ObservableCollection<Locale> Locales { get; set; } = new ObservableCollection<Locale>();

    private readonly PreferenceService _ps;
    public SettingsViewModel( PreferenceService ps)
    {
        _ps = ps;
        TTS_Pitch = _ps.TTS_Pitch;
        TTS_Volume = _ps.TTS_Volume;

        foreach(Locale locale in ps.AvailableLocales.AsQueryable().OrderBy(e => e.Language).ThenBy(e => e.Name))
        {
            Locales.Add(locale);
        }

        TTS_Locale = ps.TTS_Locale;
    }

    public void OnLeaving()
    {
        _ps.TTS_Pitch = TTS_Pitch;
        _ps.TTS_Volume = TTS_Volume;
        _ps.TTS_Locale_ID = TTS_Locale.Id;

        _ps.Save();
    }
}
