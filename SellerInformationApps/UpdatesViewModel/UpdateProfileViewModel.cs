using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using ServiceHelper.Alerts;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace SellerInformationApps.UpdatesViewModel
{
	public partial class UpdateProfileViewModel : ObservableObject
	{
		private const string DefaultProfileImage = "profilephotots.png";


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
					: DefaultProfileImage;
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"Veriler geldi, ancak ekrana yazdırılırken hata oluştu: {ex.Message}", true);
			}
		}

		[RelayCommand]
		public async Task SubmitAsync()
		{
			if (!IsFormValid(out string validationMessage))
			{
				await alertsHelper.ShowSnackBar(validationMessage, true);
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

				var httpClient = HttpClientFactory.Create("https://5462-37-130-115-91.ngrok-free.app");
				var url = "https://5462-37-130-115-91.ngrok-free.app/UserUpdateApi/EditUserData";
				string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));

				var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

				using (var request = new HttpRequestMessage(HttpMethod.Post, url))
				{
					request.Headers.TryAddWithoutValidation("Basic", authHeaderValue);
					request.Content = content;
					using (var response = await httpClient.SendAsync(request))
					{
						var responseContent = await response.Content.ReadAsStringAsync();
						var apiResponse = JsonConvert.DeserializeObject<ProfileUpdateApiResponse>(responseContent);

						if (response.IsSuccessStatusCode && apiResponse != null && apiResponse.Success)
						{
							await alertsHelper.ShowSnackBar("Profil başarıyla güncellendi", false);

							ResultData.FirstName = apiResponse.Data.FirstName;
							ResultData.LastName = apiResponse.Data.LastName;
							ResultData.UserName = apiResponse.Data.UserName;
							ResultData.Email = apiResponse.Data.Email;
							ResultData.Age = apiResponse.Data.Age;
							ResultData.ProfileImageBase64 = !string.IsNullOrWhiteSpace(apiResponse.Data.ProfileImageBase64)
								? apiResponse.Data.ProfileImageBase64
								: DefaultProfileImage;
						}
						else
						{
							await alertsHelper.ShowSnackBar("Profil güncellenemedi", true);
						}
					}
				}
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"Bir hata oluştu: {ex.Message}", true);
			}
		}

		private bool IsFormValid(out string validationMessage)
		{
			if (string.IsNullOrWhiteSpace(FirstName))
			{
				validationMessage = "Ad boş olamaz.";
				return false;
			}

			if (string.IsNullOrWhiteSpace(LastName))
			{
				validationMessage = "Soyad boş olamaz.";
				return false;
			}

			if (string.IsNullOrWhiteSpace(Email))
			{

				validationMessage = "E-posta boş olamaz.";
				return false;
			}

			if (!IsValidEmail(Email))
			{
				validationMessage = "Geçerli bir e-posta adresi giriniz.";
				return false;
			}

			if (Age == null)
			{
				validationMessage = "Yaş boş olamaz.";
				return false;
			}

			validationMessage = string.Empty;
			return true;
		}

		private UserProfileData ReadData() => new UserProfileData
		{
			FirstName = FirstName,
			LastName = LastName,
			UserName = userName,
			Email = Email,
			Age = Age,
			ProfileImageBase64 = ProfileImageBase64
		};

		private bool IsValidEmail(string email)
		{
			try
			{
				var addr = new MailAddress(email);
				return addr.Address == email;
			}
			catch
			{
				return false;
			}
		}
	}
}
