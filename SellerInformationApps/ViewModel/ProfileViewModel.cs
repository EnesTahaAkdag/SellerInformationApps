using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using ServiceHelper.Authentication;
using System.Net.Http.Headers;
using System.Text;

namespace SellerInformationApps.ViewModel
{
	public partial class ProfilePageViewModel : Authentication
	{
		[ObservableProperty] private string firstName;
		[ObservableProperty] private string lastName;
		[ObservableProperty] private string userName;
		[ObservableProperty] private string email;
		[ObservableProperty] private DateTime age;
		[ObservableProperty] private bool isLoading;

		private readonly Authentication _authentication;
		public UserProfileData UserProfileData { get; private set; }

		public ProfilePageViewModel()
		{
			_authentication = Authentication.Instance;
		}

		[RelayCommand]
		public async Task Accessed()
		{
			if (!_authentication.IsLoggedIn)
			{
				await Shell.Current.DisplayAlert("Hata", "Lütfen giriş yapın", "Tamam");
				await Shell.Current.GoToAsync("//LoginPage");
			}
			else
			{
				IsLoading = true;
				await FetchProfileDataFromApiAsync();
				IsLoading = false;
			}
		}

		private async Task FetchProfileDataFromApiAsync()
		{
			try
			{
				var userName = Preferences.Get("UserName", string.Empty);
				var password = Preferences.Get("Password", string.Empty);

				string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));

				var httpClient = HttpClientFactory.Create("https://9d96-37-130-115-34.ngrok-free.app");
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

				string url = $"/UserProfileData/DataSend?userName={userName}";

				using (var response = await httpClient.GetAsync(url))
				{
					if (response.IsSuccessStatusCode)
					{
						string json = await response.Content.ReadAsStringAsync();
						var profileData = JsonConvert.DeserializeObject<ProfileApiResponse>(json);
						if (profileData?.Success == true)
						{
							UserProfileData = new UserProfileData
							{
								FirstName = profileData.Data.FirstName,
								LastName = profileData.Data.LastName,
								UserName = profileData.Data.UserName,
								Email = profileData.Data.Email,
								Age = profileData.Data.Age ?? DateTime.MinValue
							};

							FirstName = UserProfileData.FirstName;
							LastName = UserProfileData.LastName;
							UserName = UserProfileData.UserName;
							Email = UserProfileData.Email;
							Age = UserProfileData.Age.Value;
						}
						else
						{
							await Shell.Current.DisplayAlert("Hata", $"API isteği başarısız: {profileData?.ErrorMessage}", "Tamam");
						}
					}
					else
					{
						await Shell.Current.DisplayAlert("Hata", $"API isteği başarısız: {response.StatusCode}", "Tamam");
					}
				}
			}
			catch (Exception ex)
			{
				await Shell.Current.DisplayAlert("Hata", $"Hata oluştu: {ex.Message}", "Tamam");
			}
			finally
			{
				IsLoading = false;
			}
		}

		[RelayCommand]
		public async Task LogOutAsync()
		{
			await _authentication.LogOut();
		}

		public void ClearProfileData()
		{
			FirstName = LastName = UserName = Email = string.Empty;
			Age = default;
		}
	}
}
