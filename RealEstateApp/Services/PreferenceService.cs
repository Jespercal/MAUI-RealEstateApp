using System;
namespace RealEstateApp.Services
{
	public class PreferenceService
	{
		public double TTS_Pitch { get; set; }
        public double TTS_Volume { get; set; }

        public PreferenceService()
		{
			Load();
		}

		public void Load()
		{
			TTS_Pitch = Preferences.Get("tts_pitch", 1.0);
            TTS_Volume = Preferences.Get("tts_volume", 0.5);
        }

		public void Save()
		{
			Preferences.Set("tts_pitch", TTS_Pitch);
			Preferences.Set("tts_volume", TTS_Volume);
		}
	}
}

