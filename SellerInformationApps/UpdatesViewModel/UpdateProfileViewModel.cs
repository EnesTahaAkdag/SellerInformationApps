using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using ServiceHelper.Authentication;
using System.Net.Http.Headers;
using System.Text;

namespace SellerInformationApps.UpdatesViewModel
{
	[QueryProperty(nameof(FirstName), nameof(FirstName))]
	[QueryProperty(nameof(LastName), nameof(LastName))]
	[QueryProperty(nameof(UserName), nameof(UserName))]
	[QueryProperty(nameof(Email), nameof(Email))]
	[QueryProperty(nameof(Age), nameof(Age))]
	[QueryProperty(nameof(ProfileImage), nameof(ProfileImage))]

	public partial class UpdateProfileViewModel : Authentication
	{
		[ObservableProperty] private string firstName;
		[ObservableProperty] private string lastName;
		[ObservableProperty] private string userName;
		[ObservableProperty] private string email;
		[ObservableProperty] private DateTime age;
		[ObservableProperty] private ImageSource profileImage;
		[ObservableProperty] private IFormFile formFile;


		public UserProfileData UserProfile { get; internal set; }

		public async Task WriteData(UserProfileData userProfileData)
		{
			if (userProfileData == null)
			{
				await Shell.Current.DisplayAlert("Hata", "Veriler gelmedi", "Tamam");
				return;
			}

			FirstName = userProfileData.FirstName;
			LastName = userProfileData.LastName;
			UserName = userProfileData.UserName;
			Email = userProfileData.Email;
			Age = userProfileData.Age.Value;

			if (!string.IsNullOrEmpty(userProfileData.ProfileImageBase64))
			{
				ProfileImage = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(userProfileData.ProfileImageBase64)));
			}
			else
			{
				ProfileImage = "profilephotots.png";
			}
		}

		[RelayCommand]
		public async Task SubmitAsync()
		{
			if (!IsFormValid())
			{
				await Shell.Current.DisplayAlert("Hata", "Lütfen tüm alanları doldurunuz", "Tamam");
				return;
			}

			try
			{
				var user = ReadData();
				if (user == null)
				{
					await Shell.Current.DisplayAlert("Hata", "Kullanıcı bilgileri güncellenirken bir hata oluştu", "Tamam");
					return;
				}

				var userName = Preferences.Get("UserName", string.Empty);
				var password = Preferences.Get("Password", string.Empty);

				string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));

				var httpClient = HttpClientFactory.Create("https://bd1b-37-130-115-91.ngrok-free.app/");
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

				string url = "/UserUpdateApi/EditUserData";
				var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

				using (var response = await httpClient.PostAsync(url, content))
				{
					await HandleResponseAsync(response);
				}
			}
			catch (Exception ex)
			{
				await Shell.Current.DisplayAlert("Hata", $"Hata oluştu: {ex.Message}", "Tamam");
			}
		}

		private bool IsFormValid()
		{
			return !(string.IsNullOrWhiteSpace(FirstName) ||
					 string.IsNullOrWhiteSpace(LastName) ||
					 string.IsNullOrWhiteSpace(UserName) ||
					 string.IsNullOrWhiteSpace(Email) ||
					 Age == default);
		}

		private UserProfileData ReadData()
		{
			return new UserProfileData
			{
				FirstName = FirstName,
				LastName = LastName,
				UserName = UserName,
				Email = Email,
				Age = Age
			};
		}

		private async Task HandleResponseAsync(HttpResponseMessage response)
		{
			if (response.IsSuccessStatusCode)
			{
				string json = await response.Content.ReadAsStringAsync();
				var apiResponse = JsonConvert.DeserializeObject<UserApiResponse>(json);

				if (apiResponse?.Success == true)
				{
					await Shell.Current.DisplayAlert("Başarılı", "Güncelleme başarılı", "Tamam");
				}
				else
				{
					await Shell.Current.DisplayAlert("Hata", "Güncelleme başarısız oldu", "Tamam");
				}
			}
			else
			{
				await Shell.Current.DisplayAlert("Hata", $"Http isteği başarısız: {response.StatusCode}", "Tamam");
			}
		}
	}
}
