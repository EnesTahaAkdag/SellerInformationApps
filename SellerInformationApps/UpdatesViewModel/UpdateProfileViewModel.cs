using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using ServiceHelper.Alerts;
using System.Net.Http.Headers;
using System.Text;

namespace SellerInformationApps.UpdatesViewModel
{
	public partial class UpdateProfileViewModel : ObservableObject
	{
		public UserProfileData ResultData { get; private set; } = new UserProfileData();
		private readonly string userName = Preferences.Get("UserName", string.Empty);

		[ObservableProperty] private string firstName;
		[ObservableProperty] private string lastName;
		[ObservableProperty] private string email;
		[ObservableProperty] private DateTime? age;
		[ObservableProperty] private string profileImageBase64;

		private readonly AlertsHelper alertsHelper = new();
		private readonly Popup _popup;

		public UpdateProfileViewModel(Popup popup)
		{
			_popup = popup;
		}

		public async Task LoadDataAsync(UserProfileData updateUserData)
		{
			if (updateUserData == null)
			{
				await alertsHelper.ShowSnackBar("Güncellenen veriler Profil Sayfasına Gelmedi", true);
				return;
			}

			try
			{
				FirstName = updateUserData.FirstName ?? string.Empty;
				LastName = updateUserData.LastName ?? string.Empty;
				Email = updateUserData.Email ?? string.Empty;
				Age = updateUserData.Age ?? DateTime.Now;
				ProfileImageBase64 = !string.IsNullOrWhiteSpace(updateUserData.ProfileImageBase64)
					? updateUserData.ProfileImageBase64
					: "profilephotots.png";
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"Veriler geldi, ancak ekrana yazdırılırken hata oluştu: {ex.Message}", true);
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
				var password = Preferences.Get("Password", string.Empty);

				if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
				{
					await alertsHelper.ShowSnackBar("Kullanıcı adı veya şifre eksik", true);
					return;
				}

				var client = HttpClientFactory.Create("https://48d6-37-130-115-91.ngrok-free.app");
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}")));

				var url = "https://48d6-37-130-115-91.ngrok-free.app/UserUpdateApi/EditUserData";
				var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

				using (var response = await client.PostAsync(url, content))
				{
					var responseContent = await response.Content.ReadAsStringAsync();
					var apiResponse = JsonConvert.DeserializeObject<ProfileUpdateApiResponse>(responseContent);

					if (response.IsSuccessStatusCode && apiResponse != null && apiResponse.Success)
					{
						await alertsHelper.ShowSnackBar("Profil başarıyla güncellendi", false);
						UpdateResultData(apiResponse.Data);
					}
					else
					{
						await alertsHelper.ShowSnackBar("Profil güncellenemedi", true);
					}
				}
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"Bir hata oluştu: {ex.Message}", true);
			}
		}

		private bool IsFormValid() =>
			!string.IsNullOrWhiteSpace(FirstName) &&
			!string.IsNullOrWhiteSpace(LastName) &&
			!string.IsNullOrWhiteSpace(Email) &&
			Age != null;

		private UserProfileData ReadData() => new UserProfileData
		{
			FirstName = FirstName,
			LastName = LastName,
			UserName = userName,
			Email = Email,
			Age = Age
		};

		private void UpdateResultData(UserProfileData updatedData)
		{
			ResultData.FirstName = updatedData.FirstName;
			ResultData.LastName = updatedData.LastName;
			ResultData.UserName = updatedData.UserName;
			ResultData.Email = updatedData.Email;
			ResultData.Age = updatedData.Age;
			ResultData.ProfileImageBase64 = !string.IsNullOrWhiteSpace(updatedData.ProfileImageBase64)
				? updatedData.ProfileImageBase64
				: "profilephotots.png";
		}
	}
}
