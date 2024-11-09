using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using ServiceHelper.Alerts;
using System.Net.Mail;
using System.Text;

namespace SellerInformationApps.UpdatesViewModel
{
	public partial class UpdateProfileViewModel : ObservableObject
	{
		private const string DefaultProfileImage = "profilephotots.png";

		public UserProfileData ResultData { get; private set; } = new UserProfileData();
		private readonly string userName = Preferences.Get("UserName", string.Empty);
		private readonly string password = Preferences.Get("Password", string.Empty);

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
				user.ProfileImageBase64 = string.IsNullOrWhiteSpace(ProfileImageBase64) ? DefaultProfileImage : ProfileImageBase64;

				var httpClient = HttpClientFactory.Create("https://1304-37-130-115-91.ngrok-free.app");
				string url = "https://1304-37-130-115-91.ngrok-free.app/UserUpdateApi/EditUserData";
				var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));

				using (var request = new HttpRequestMessage(HttpMethod.Post, url))
				{
					request.Headers.TryAddWithoutValidation("Authorization", $"Basic {authHeaderValue}");
					request.Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
					using (var response = await httpClient.SendAsync(request))
					{
						var responseContent = await response.Content.ReadAsStringAsync();
						var apiResponse = JsonConvert.DeserializeObject<ProfileUpdateApiResponse>(responseContent);

						if (response.IsSuccessStatusCode && apiResponse?.Success == true)
						{
							ResultData = apiResponse.Data;
							await alertsHelper.ShowSnackBar("Profil başarıyla güncellendi", false);
						}
						else
						{
							await alertsHelper.ShowSnackBar(apiResponse?.ErrorMessage ?? "Profil güncellenemedi", true);
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
			else
			{
				if (!IsValidEmail(Email))
				{
					validationMessage = "Geçerli bir e-posta adresi giriniz.";
					return false;
				}
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
				string host = addr.Host;

				string[] validTlds = { ".com", ".net", ".org", ".edu", ".gov", ".co", ".us", ".uk" };

				return validTlds.Any(tld => host.EndsWith(tld, StringComparison.OrdinalIgnoreCase));
			}
			catch
			{
				return false;
			}
		}
	}
}
