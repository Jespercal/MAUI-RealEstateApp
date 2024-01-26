using System;
namespace RealEstateApp.Services
{
	public class SettingsService
	{
        public SettingsService()
		{
			Task.Run(Load);
		}

		public async void Load()
		{

		}
	}
}

