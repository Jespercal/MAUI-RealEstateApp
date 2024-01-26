using System;
namespace RealEstateApp.Services
{
	public class PreferenceService
	{
		public double TTS_Pitch { get; set; }
        public double TTS_Volume { get; set; }
		public string TTS_Locale_ID { get; set; }
		public Locale TTS_Locale => AvailableLocales.FirstOrDefault(e => e.Id == TTS_Locale_ID) ?? _availableLocales.First(e => e.Language == "en-US");

        private IEnumerable<Locale> _availableLocales;
		public IEnumerable<Locale> AvailableLocales => _availableLocales;

        public PreferenceService()
		{
			Task.Run(Load);
		}

		public async void Load()
		{
			_availableLocales = await TextToSpeech.Default.GetLocalesAsync();

            TTS_Pitch = Preferences.Get("tts_pitch", 1.0);
            TTS_Volume = Preferences.Get("tts_volume", 0.5);
            TTS_Locale_ID = Preferences.Get("tts_locale", _availableLocales.First(e => e.Language == "en-US").Id);
        }

		public void Save()
		{
			Preferences.Set("tts_pitch", TTS_Pitch);
			Preferences.Set("tts_volume", TTS_Volume);
			Preferences.Set("tts_locale", TTS_Locale_ID);
		}
	}
}

