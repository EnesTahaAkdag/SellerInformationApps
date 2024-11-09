using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using ServiceHelper.Authentication;
using System.Net.Http.Headers;
using System.Text;
using ServiceHelper.Alerts;

namespace SellerInformationApps.ViewModel
{
	public partial class ProfilePageViewModel : Authentication
	{
		private readonly AlertsHelper _alertsHelper = new();
		private readonly Authentication _authentication;

		[ObservableProperty] public string firstName;
		[ObservableProperty] public string lastName;
		[ObservableProperty] public string userName;
		[ObservableProperty] public string email;
		[ObservableProperty] public DateTime? age;
		[ObservableProperty] public string profileImageBase64;
		[ObservableProperty] private bool isLoading;

		public UserProfileData UserProfileData { get; set; }
		public UserProfilePhoto UserProfilePhoto { get; set; }

		public ProfilePageViewModel()
		{
			_authentication = Authentication.Instance;
			UserProfilePhoto = new UserProfilePhoto();
		}

		public void ClearProfileData()
		{
			FirstName = LastName = UserName = Email = string.Empty;
			Age = null;
			ProfileImageBase64 = null;
			UserProfilePhoto = new UserProfilePhoto();
		}

		public async Task WriteData(UserProfileData updateUserData)
		{
			if (updateUserData == null)
			{
				await _alertsHelper.ShowSnackBar("Güncellenen veriler Profil Sayfasına Gelmedi", true);
			}
			else
			{
				try
				{
					FirstName = updateUserData.FirstName ?? string.Empty;
					LastName = updateUserData.LastName ?? string.Empty;
					UserName = updateUserData.UserName ?? string.Empty;
					Email = updateUserData.Email ?? string.Empty;
					Age = updateUserData.Age;
					ProfileImageBase64 = updateUserData.ProfileImageBase64 ?? string.Empty;
				}
				catch (Exception ex)
				{
					await _alertsHelper.ShowSnackBar($"Veriler geldi ama ekrana basılırken hata verdi: {ex.Message}", true);
				}
			}
		}

		[RelayCommand]
		public async Task AccessedAsync()
		{
			if (!_authentication.IsLoggedIn)
			{
				await _alertsHelper.ShowSnackBar("Lütfen giriş yapın", true);
				await Shell.Current.GoToAsync("//LoginPage");
				return;
			}

			if (IsProfileDataEmpty())
			{
				await FetchProfileDataFromApiAsync();
			}
		}

		private bool IsProfileDataEmpty()
		{
			return string.IsNullOrEmpty(FirstName) &&
				   string.IsNullOrEmpty(LastName) &&
				   string.IsNullOrEmpty(UserName) &&
				   string.IsNullOrEmpty(Email) &&
				   Age == null &&
				   ProfileImageBase64 == null;
		}

		public async Task FetchProfileDataFromApiAsync()
		{
			try
			{
				IsLoading = true;

				var userName = Preferences.Get("UserName", string.Empty);
				var password = Preferences.Get("Password", string.Empty);

				if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
				{
					await _alertsHelper.ShowSnackBar("Kullanıcı adı veya şifre boş olamaz.", true);
					return;
				}

				string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));
				var httpClient = HttpClientFactory.Create("https://1304-37-130-115-91.ngrok-free.app");
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

				string url = $"https://1304-37-130-115-91.ngrok-free.app/UserDataSendApi/DataSend?userName={userName}";

				using (var request = new HttpRequestMessage(HttpMethod.Get, url))
				{
					using (var response = await httpClient.SendAsync(request))
					{
						if (response.IsSuccessStatusCode)
						{
							var json = await response.Content.ReadAsStringAsync();
							var profileData = JsonConvert.DeserializeObject<ProfileApiResponse>(json);

							if (profileData?.Success == true)
							{
								FirstName = profileData.Data.FirstName ?? string.Empty;
								LastName = profileData.Data.LastName ?? string.Empty;
								UserName = profileData.Data.UserName ?? string.Empty;
								Email = profileData.Data.Email ?? string.Empty;
								Age = profileData.Data.Age;
								UserProfileData = profileData.Data;

								if (!string.IsNullOrEmpty(profileData.Data.ProfileImageBase64))
								{
									ProfileImageBase64 = profileData.Data.ProfileImageBase64;
									UserProfilePhoto.ProfileImageBase64 = profileData.Data.ProfileImageBase64;
								}
								else
								{
									ProfileImageBase64 = "profilephotots.png";
									UserProfilePhoto.ProfileImageBase64 = "profilephotots.png";
								}
							}
							else
							{
								await _alertsHelper.ShowSnackBar($"API isteği başarısız: {profileData?.ErrorMessage}", true);
							}
						}
						else
						{
							await _alertsHelper.ShowSnackBar($"API isteği başarısız: {response.StatusCode}", true);
						}
					}
				}
			}
			catch (Exception ex)
			{
				await _alertsHelper.ShowSnackBar($"Hata oluştu: {ex.Message}", true);
			}
			finally
			{
				IsLoading = false;
			}
		}

		[RelayCommand]
		public async Task LogOutAsync()
		{
			ClearProfileData();
			await _authentication.LogOut();
		}
	}
}
