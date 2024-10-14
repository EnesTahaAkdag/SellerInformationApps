using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using ServiceHelper.Authentication;
using System.Net.Http.Headers;
using System.Text;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Alerts;

namespace SellerInformationApps.ViewModel
{
	public partial class ProfilePageViewModel : Authentication
	{
		[ObservableProperty]
		private string firstName;

		[ObservableProperty]
		private string lastName;

		[ObservableProperty]
		private string userName;

		[ObservableProperty]
		private string email;

		[ObservableProperty]
		private DateTime? age;

		[ObservableProperty]
		private ImageSource profileImage;

		[ObservableProperty]
		private bool isLoading;

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
				await ShowToast("Lütfen giriş yapın");
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

				var httpClient = HttpClientFactory.Create("https://c846-37-130-115-91.ngrok-free.app/");
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

				string url = $"/UserDataSendApi/DataSend?userName={userName}";

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
								Age = profileData.Data.Age,
								ProfileImageBase64 = profileData.Data.ProfileImageBase64
							};

							FirstName = UserProfileData.FirstName;
							LastName = UserProfileData.LastName;
							UserName = UserProfileData.UserName;
							Email = UserProfileData.Email;
							Age = UserProfileData.Age;

							if (!string.IsNullOrEmpty(UserProfileData.ProfileImageBase64))
							{
								ProfileImage = ImageSource.FromUri(new Uri(UserProfileData.ProfileImageBase64));
							}
							else
							{
								ProfileImage = "profilephotots.png";
							}
						}
						else
						{
							await ShowToast($"API isteği başarısız: {profileData?.ErrorMessage}");
						}
					}
					else
					{
						await ShowToast($"API isteği başarısız: {response.StatusCode}");
					}
				}
			}
			catch (Exception ex)
			{
				await ShowToast($"Hata oluştu: {ex.Message}");
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
			Age = null;
			ProfileImage = "profilephotots.png";
		}

		private async Task ShowToast(string message, bool isSuccess = false)
		{
			var toast = Toast.Make(message, ToastDuration.Short, isSuccess ? 20 : 14);

			await MainThread.InvokeOnMainThreadAsync(async () =>
			{
				await toast.Show();
			});
		}
	}
}
