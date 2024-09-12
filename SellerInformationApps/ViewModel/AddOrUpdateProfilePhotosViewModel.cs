using System.Net.Http.Headers;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using ServiceHelper.Authentication;

namespace SellerInformationApps.ViewModel
{
	public partial class AddOrUpdateProfilePhotosViewModel : Authentication
	{
		[ObservableProperty]
		private string userName = Preferences.Get("UserName", string.Empty);

		private Stream _profileImageStream;

		public void SetProfileImageStream(Stream stream)
		{
			_profileImageStream = stream;
		}

		public async Task AddOrUpdateProfilePhotosAsync(Stream stream)
		{
			SetProfileImageStream(stream);

			if (_profileImageStream != null && _profileImageStream.Length > 0)
			{
				try
				{
					using var httpClient = HttpClientFactory.Create("https://8b27-37-130-115-34.ngrok-free.app");
					string url = "https://8b27-37-130-115-34.ngrok-free.app/UserUpdateAPI/UpdateProfileImage";

					var formData = new MultipartFormDataContent();

					string uniqueFileName = $"{Guid.NewGuid()}.jpg";

					var fileContent = new StreamContent(_profileImageStream);
					fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
					formData.Add(fileContent, "ProfilePhoto", uniqueFileName);

					formData.Add(new StringContent(UserName), "userName");

					using (var response = await httpClient.PostAsync(url, formData))
					{
						string responseContent = await response.Content.ReadAsStringAsync();

						if (!response.IsSuccessStatusCode)
						{
							await Shell.Current.DisplayAlert("Hata", $"HTTP İsteği Başarısız: {response.StatusCode}, İçerik: {responseContent}", "Tamam");
						}
						else
						{
							await HandleResponseAsync(response);
						}
					};
				}
				catch (Exception ex)
				{
					await Shell.Current.DisplayAlert("Hata", ex.Message, "Tamam");
				}
			}
			else
			{
				await Shell.Current.DisplayAlert("Hata", "Henüz bir resim seçilmedi veya resim geçersiz.", "Tamam");
			}
		}


		private async Task HandleResponseAsync(HttpResponseMessage response)
		{
			if (response.IsSuccessStatusCode)
			{
				string json = await response.Content.ReadAsStringAsync();
				var profilePhotosApiResponse = JsonConvert.DeserializeObject<ProfilePohotosApiResponse>(json);

				if (profilePhotosApiResponse?.Success == true)
				{
					await Shell.Current.DisplayAlert("Başarılı", "Resminiz Güncellendi", "Tamam");
				}
				else
				{
					await Shell.Current.DisplayAlert("Hata", profilePhotosApiResponse?.ErrorMessage ?? "Bilinmeyen hata", "Tamam");
				}
			}
			else
			{
				await Shell.Current.DisplayAlert("Hata", $"HTTP İsteği Başarısız: {response.StatusCode}", "Tamam");
			}
		}
	}
}
