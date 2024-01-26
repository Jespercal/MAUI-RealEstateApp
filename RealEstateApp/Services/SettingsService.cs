using System.Text.Json;
using RealEstateApp.Models;

namespace RealEstateApp.Services
{
	public class SettingsService
	{
		public LoginResult LoginDetails { get; set; } = new LoginResult();
		public string LastUsername { get; set; } = String.Empty;

        public SettingsService()
		{
			Task.Run(Load);
		}

		public async Task LogoutAsync()
		{
			LoginDetails = new LoginResult();
			LastUsername = LastUsername ?? "";
			await Save();
		}

		public async Task<int> TryLoginAsync( string username, string password )
		{
			if(username == "admin" && password == "password")
			{
				LoginDetails.AccessToken = "some-access-token";
				LoginDetails.RefreshToken = "some-refresh-token";
				LoginDetails.Succeeded = true;
				LastUsername = username;
				await Save();
				return 1;
			}

            LoginDetails.AccessToken = "";
            LoginDetails.RefreshToken = "";
            LoginDetails.Succeeded = false;
            LastUsername = username;
            await Save();
            return 0;
        }

		public async Task Load()
		{
			string rawLoginDetails = await SecureStorage.GetAsync("login_details");

			if(rawLoginDetails == null || rawLoginDetails == "")
			{
				LoginDetails = new LoginResult();
				return;
			}

			LoginDetails = JsonSerializer.Deserialize<LoginResult>(rawLoginDetails);

			LastUsername = await SecureStorage.GetAsync("last_username");
		}

		public async Task Save()
		{
			await SecureStorage.SetAsync("login_details",JsonSerializer.Serialize(LoginDetails) ?? "");
			await SecureStorage.SetAsync("last_username", LastUsername ?? "");
		}
	}
}

