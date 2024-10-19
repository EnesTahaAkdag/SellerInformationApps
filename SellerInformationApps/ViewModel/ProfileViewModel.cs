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
		private readonly AlertsHelper _alertsHelper = new AlertsHelper();
		private readonly Authentication _authentication;

		[ObservableProperty] private string firstName;
		[ObservableProperty] private string lastName;
		[ObservableProperty] private string userName;
		[ObservableProperty] private string email;
		[ObservableProperty] private DateTime? age;
		[ObservableProperty] private ImageSource profileImage;
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
			ProfileImage = null;
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
				}
				catch (Exception ex)
				{
					await _alertsHelper.ShowSnackBar($"Veriler Geldi Ama Ekrana Basılırken hata verdi: {ex.Message}", true);
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
				   ProfileImage == null;
		}

		private async Task FetchProfileDataFromApiAsync()
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
				var httpClient = HttpClientFactory.Create("https://59b7-37-130-115-91.ngrok-free.app");
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

				string url = $"/UserDataSendApi/DataSend?userName={userName}";
				using (var response = await httpClient.GetAsync(url))
				{
					if (response.IsSuccessStatusCode)
					{
						var json = await response.Content.ReadAsStringAsync();
						var profileData = JsonConvert.DeserializeObject<ProfileApiResponse>(json);

						if (profileData?.Success == true)
						{
							UpdateProfileData(profileData.Data);
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
			catch (Exception ex)
			{
				await _alertsHelper.ShowSnackBar($"Hata oluştu: {ex.Message}", true);
			}
			finally
			{
				IsLoading = false;
			}
		}

		private void UpdateProfileData(UserProfileData data)
		{
			if (data == null)
			{
				return;
			}

			UserProfileData = data;

			FirstName = data.FirstName ?? string.Empty;
			LastName = data.LastName ?? string.Empty;
			UserName = data.UserName ?? string.Empty;
			Email = data.Email ?? string.Empty;
			Age = data.Age;

			if (UserProfilePhoto == null)
			{
				UserProfilePhoto = new UserProfilePhoto();
			}

			if (!string.IsNullOrEmpty(data.ProfileImageBase64))
			{
				ProfileImage = ImageSource.FromUri(new Uri(data.ProfileImageBase64));
				UserProfilePhoto.ProfileImageSource = data.ProfileImageBase64;
			}
			else
			{
				UserProfilePhoto.ProfileImageSource = "profilephotots.png";
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
