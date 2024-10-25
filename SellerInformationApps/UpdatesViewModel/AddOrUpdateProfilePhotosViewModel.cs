using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using SellerInformationApps.Models;
using System.Net.Http.Headers;
using ServiceHelper.Authentication;
using System.Text;
using ServiceHelper.Alerts;
using PraPazar.ServiceHelper;

namespace SellerInformationApps.UpdatesViewModel
{
	public partial class AddOrUpdateProfilePhotosViewModel : Authentication
	{
		public string resultData { get; set; }

		private readonly AlertsHelper _alertsHelper = new AlertsHelper();

		[ObservableProperty]
		private string userName = Preferences.Get("UserName", string.Empty);

		[ObservableProperty]
		public string profileImageBase64;

		public AddOrUpdateProfilePhotosViewModel()
		{
		}

		public async Task WriteData(string photo)
		{
			if (string.IsNullOrEmpty(photo))
			{
				ProfileImageBase64 = "profilephotots.png";
				await _alertsHelper.ShowSnackBar("Profil resmi gelmedi veya boş.", true);
			}
			else
			{
				try
				{
					ProfileImageBase64 = photo;
				}
				catch (Exception ex)
				{
					ProfileImageBase64 = "profilephotots.png";
					await _alertsHelper.ShowSnackBar($"Hata oluştu: {ex.Message}", true);
				}
			}
		}

		public async Task AddOrUpdateProfilePhotosAsync()
		{
			if (!IsFormValid())
			{
				await _alertsHelper.ShowSnackBar("Resim boş kaydedilemez.", true);
				return;
			}

			try
			{
				var newPhoto = CreateProfilePhotoModel();

				string password = Preferences.Get("Password", string.Empty);
				string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{UserName}:{password}"));

				var httpClient = HttpClientFactory.Create("https://48d6-37-130-115-91.ngrok-free.app");
				string url = "https://48d6-37-130-115-91.ngrok-free.app/UserUpdateApi/UpdateUserProfileImage";

				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

				var content = new StringContent(JsonConvert.SerializeObject(newPhoto), Encoding.UTF8, "application/json");

				using (var response = await httpClient.PostAsync(url, content))
				{
					if (response.IsSuccessStatusCode)
					{
						string responseContent = await response.Content.ReadAsStringAsync();
						var apiResponse = JsonConvert.DeserializeObject<ProfilePohotosApiResponse>(responseContent);

						if (apiResponse?.Success == true)
						{
							resultData = apiResponse.Data.NewProfileImageBase64;
							await _alertsHelper.ShowSnackBar("Resminiz güncellendi.");
						}
						else
						{
							await _alertsHelper.ShowSnackBar(apiResponse?.ErrorMessage ?? "Bilinmeyen hata.", true);
						}
					}
					else
					{
						await _alertsHelper.ShowSnackBar($"Sunucu hatası: {response.StatusCode}", true);
						return;
					}

				}
			}
			catch (Exception ex)
			{
				await _alertsHelper.ShowSnackBar($"Bir hata oluştu: {ex.Message}", true);
			}
		}

		private bool IsFormValid()
		{
			return !string.IsNullOrEmpty(ProfileImageBase64);
		}

		private ProfilePhotoModel CreateProfilePhotoModel()
		{
			return new ProfilePhotoModel
			{
				UserName = UserName,
				NewProfileImageBase64 = ProfileImageBase64
			};
		}
	}
}
