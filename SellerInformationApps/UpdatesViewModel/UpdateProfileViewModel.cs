using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using ServiceHelper.Alerts;
using ServiceHelper.Authentication;
using System.Net.Http.Headers;
using System.Text;

namespace SellerInformationApps.UpdatesViewModel
{
	public partial class UpdateProfileViewModel : Authentication
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
		private ImageSource profileImage;

		public AlertsHelper alertsHelper = new AlertsHelper();


		public AddOrUpdateProfilePhotosViewModel ProfilePhotosViewModel { get; set; }

		public UserProfileData UserProfile { get; internal set; }

		public async Task WriteData(UserProfileData userProfileData)
		{
			if (userProfileData == null)
			{
				await alertsHelper.ShowSnackBar("Veriler gelmedi", true);
				return;
			}

			FirstName = userProfileData.FirstName;
			LastName = userProfileData.LastName;
			UserName = userProfileData.UserName;
			Email = userProfileData.Email;
			Age = userProfileData.Age.Value;

			if (!string.IsNullOrEmpty(userProfileData.ProfileImageBase64))
			{
				try
				{
					ProfileImage = ImageSource.FromUri(new Uri(userProfileData.ProfileImageBase64));
				}
				catch (Exception ex)
				{
					ProfileImage = "profilephotots.png";
					await alertsHelper.ShowSnackBar($"Profil resmi yüklenirken hata: {ex.Message}", true);
				}
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
				await alertsHelper.ShowSnackBar("Lütfen tüm alanları doldurunuz", true);
				return;
			}

			try
			{
				var user = ReadData();
				if (user == null)
				{
					await alertsHelper.ShowSnackBar("Kullanıcı bilgileri güncellenirken bir hata oluştu", true);
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
				await alertsHelper.ShowSnackBar($"Hata oluştu: {ex.Message}", true);
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
					await alertsHelper.ShowSnackBar("Güncelleme başarılı");
				}
				else
				{
					await alertsHelper.ShowSnackBar("Güncelleme başarısız oldu", true);
				}
			}
			else
			{
				await alertsHelper.ShowSnackBar($"Http isteği başarısız: {response.StatusCode}", true);
			}
		}
	}
}
