using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using SellerInformationApps.Models;
using System.Net.Http.Headers;
using ServiceHelper.Authentication;
using System.Text;
using PraPazar.ServiceHelper;

namespace SellerInformationApps.UpdatesViewModel
{
	public partial class AddOrUpdateProfilePhotosViewModel : Authentication
	{
		[ObservableProperty]
		private string userName = Preferences.Get("UserName", string.Empty);

		[ObservableProperty]
		private ImageSource profileImage = ImageSource.FromFile("profilephotos");

		partial void OnProfileImageChanged(ImageSource value)
		{
			if (ProfileImage == null)
			{
				ProfileImage = ImageSource.FromFile("profilephotos.png");
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

				var password = Preferences.Get("Password", string.Empty);
				string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{UserName}:{password}"));

				string url = "https://0ad8-37-130-115-91.ngrok-free.app/UserUpdateApi/UpdateUserProfileImage";

				var client = HttpClientFactory.Create(url);
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
							await Shell.Current.DisplayAlert("Hata", $"Sunucu hatası: {response.StatusCode}", "Tamam");
							return;
						}

						string responseContent = await response.Content.ReadAsStringAsync();
						await HandleResponseAsync(responseContent);
					}
				}
			}
			catch (Exception ex)
			{
				await Shell.Current.DisplayAlert("Hata", $"Bir hata oluştu: {ex.Message}", "Tamam");
			}
		}

		private async Task HandleResponseAsync(string responseContent)
		{
			try
			{
				var profilePhotosApiResponse = JsonConvert.DeserializeObject<ProfilePohotosApiResponse>(responseContent);

				if (profilePhotosApiResponse?.Success == true)
				{
					await Shell.Current.DisplayAlert("Başarılı", "Resminiz Güncellendi", "Tamam");
				}
				else
				{
					await Shell.Current.DisplayAlert("Hata", profilePhotosApiResponse?.ErrorMessage ?? "Bilinmeyen hata", "Tamam");
				}
			}
			catch (Exception ex)
			{
				await Shell.Current.DisplayAlert("Hata", ex.Message, "Tamam");
			}
		}
	}
}
