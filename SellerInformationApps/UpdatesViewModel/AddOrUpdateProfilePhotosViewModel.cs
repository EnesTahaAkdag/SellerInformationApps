using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using SellerInformationApps.Models;
using System.Net.Http.Headers;
using ServiceHelper.Authentication;
using System.Text;
using PraPazar.ServiceHelper;
using Microsoft.AspNetCore.Http;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Alerts;

namespace SellerInformationApps.UpdatesViewModel
{
	public partial class AddOrUpdateProfilePhotosViewModel : Authentication
	{
		[ObservableProperty]
		private string userName = Preferences.Get("UserName", string.Empty);

		[ObservableProperty]
		private ImageSource profileImage;

		

		public async Task AddOrUpdateProfilePhotosAsync(Stream imageStream)
		{
			try
			{
				if (imageStream.CanSeek)
				{
					imageStream.Position = 0;
				}

				var password = Preferences.Get("Password", string.Empty);
				string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{UserName}:{password}"));

				string url = "https://c846-37-130-115-91.ngrok-free.app/UserUpdateApi/UpdateUserProfileImage";

				var client = HttpClientFactory.Create("https://c846-37-130-115-91.ngrok-free.app");
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
							await	ShowToast($"Sunucu hatası: {response.StatusCode}	");
							return;
						}

						string responseContent = await response.Content.ReadAsStringAsync();
						await HandleResponseAsync(responseContent);
					}
				}
			}
			catch (Exception ex)
			{
				await ShowToast($"Bir hata oluştu: {ex.Message}");
			}
		}

		private async Task HandleResponseAsync(string responseContent)
		{
			try
			{
				var profilePhotosApiResponse = JsonConvert.DeserializeObject<ProfilePohotosApiResponse>(responseContent);

				if (profilePhotosApiResponse?.Success == true)
				{
					await ShowToast("Resminiz Güncellendi");
				}
				else
				{
					await ShowToast(profilePhotosApiResponse?.ErrorMessage ?? "Bilinmeyen hata");
				}
			}
			catch (Exception ex)
			{
				await ShowToast(ex.Message);
			}
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
