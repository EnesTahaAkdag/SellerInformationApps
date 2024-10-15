using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using SellerInformationApps.Models;
using System.Net.Http.Headers;
using ServiceHelper.Authentication;
using System.Text;
using PraPazar.ServiceHelper;
using ServiceHelper.Alerts;
using SellerInformationApps.Services;

namespace SellerInformationApps.UpdatesViewModel
{
	public partial class AddOrUpdateProfilePhotosViewModel : Authentication
	{
		private readonly AlertsHelper _alertsHelper = new AlertsHelper();

		[ObservableProperty]
		private string userName = Preferences.Get("UserName", string.Empty);

		[ObservableProperty]
		public ImageSource profileImage;

		public async Task WriteData(UserProfilePhoto userProfilePhoto)
		{
			if (userProfilePhoto == null)
			{
				await _alertsHelper.ShowSnackBar("Profil resmi gelmedi", true);
			}
			else
			{
				try
				{
					ProfileImage = ImageSource.FromUri(new Uri(userProfilePhoto.ProfileImageSource));
				}
				catch (Exception ex)
				{
					ProfileImage = "profilephotots.png";
					await _alertsHelper.ShowSnackBar($"Hata oluştu: {ex.Message}", true);
				}
			}
		}

		public async Task AddOrUpdateProfilePhotosAsync(Stream imageStream)
		{
			try
			{
				if (imageStream.CanSeek)
				{
					imageStream.Position = 0;
				}

				string password = Preferences.Get("Password", string.Empty);
				string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{UserName}:{password}"));

				string url = "https://a8c0-37-130-115-91.ngrok-free.app/UserUpdateApi/UpdateUserProfileImage";

				var client = HttpClientFactory.Create("https://a8c0-37-130-115-91.ngrok-free.app");
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

				using (var content = new MultipartFormDataContent())
				{
					var streamContent = new StreamContent(imageStream);
					streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

					content.Add(streamContent, "ProfileImage", "profileImage.jpg");
					content.Add(new StringContent(UserName), "UserName");

					using (var response = await client.PostAsync(url, content))
					{
						if (!response.IsSuccessStatusCode)
						{
							await _alertsHelper.ShowSnackBar($"Sunucu hatası: {response.StatusCode}", true);
							return;
						}

						string responseContent = await response.Content.ReadAsStringAsync();
						await HandleResponseAsync(responseContent);
					}
				}
			}
			catch (Exception ex)
			{
				await _alertsHelper.ShowSnackBar($"Bir hata oluştu: {ex.Message}", true);
			}
		}

		private async Task HandleResponseAsync(string responseContent)
		{
			try
			{
				var profilePhotosApiResponse = JsonConvert.DeserializeObject<ProfilePohotosApiResponse>(responseContent);

				if (profilePhotosApiResponse?.Success == true)
				{
					await _alertsHelper.ShowSnackBar("Resminiz güncellendi");
				}
				else
				{
					await _alertsHelper.ShowSnackBar(profilePhotosApiResponse?.ErrorMessage ?? "Bilinmeyen hata", true);
				}
			}
			catch (Exception ex)
			{
				await _alertsHelper.ShowSnackBar(ex.Message, true);
			}
		}
	}
}
