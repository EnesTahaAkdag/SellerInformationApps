using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using ServiceHelper.Authentication;
using System.Net.Http.Headers;
using System.Text;

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
	private DateTime age;

	[ObservableProperty]
	private bool isLoading;

	private readonly Authentication _authentication;

	private UserProfileData userProfileData;

	public ProfilePageViewModel()
	{
		_authentication = Authentication.Instance;
	}

	[RelayCommand]
	public async Task Accessed()
	{
		if (!_authentication.IsLoggedIn)
		{
			await Shell.Current.DisplayAlert("Hata", "Lütfen Giriş Yapın", "Tamam");
			await Shell.Current.GoToAsync("//LoginPage");
		}
		else
		{
			IsLoading = true;
			await FetchProfileDataFromApiAsync();
			IsLoading = false;
		}
	}

	public async Task FetchProfileDataFromApiAsync()
	{
		try
		{
			var userName = Preferences.Get("UserName", string.Empty);
			var password = Preferences.Get("Password", string.Empty);

			string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));

			var httpClient = HttpClientFactory.Create("https://f038-37-130-115-34.ngrok-free.app");
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

			string url = $"/UserProfileData/DataSend?userName={userName}";

			using (var response = await httpClient.GetAsync(url))
			{
				if (response.IsSuccessStatusCode)
				{
					string json = await response.Content.ReadAsStringAsync();
					var profileData = JsonConvert.DeserializeObject<ProfileApiResponse>(json);
					if (profileData.Success)
					{
						userProfileData = new UserProfileData
						{
							FirstName = profileData.Data.FirstName,
							LastName = profileData.Data.LastName,
							UserName = profileData.Data.UserName,
							Email = profileData.Data.Email,
							Age = profileData.Data.Age.HasValue ? profileData.Data.Age.Value : DateTime.MinValue
						};

						FirstName = userProfileData.FirstName;
						LastName = userProfileData.LastName;
						UserName = userProfileData.UserName;
						Email = userProfileData.Email;
						Age = userProfileData.Age.Value;
					}
					else
					{
						await Shell.Current.DisplayAlert("Hata", $"API İsteği Başarısız: {profileData.ErrorMessage}", "Tamam");
					}
				}
				else
				{
					await Shell.Current.DisplayAlert("Hata", $"API İsteği Başarısız: {response.StatusCode}", "Tamam");
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

	[RelayCommand]
	public async Task UpdateProfileAsync()
	{
		if (userProfileData != null)
		{
			await Shell.Current.GoToAsync("//UpdateProfilePage", new Dictionary<string, object>
			{
				{ "UserProfileData", userProfileData }
			});
		}
		else
		{
			await Shell.Current.DisplayAlert("Hata", "Profil bilgileri yüklenemedi", "Tamam");
		}
	}
}
