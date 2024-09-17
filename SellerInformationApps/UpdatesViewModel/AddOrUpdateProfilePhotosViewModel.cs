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

		private readonly HttpClient _httpClient;

		public AddOrUpdateProfilePhotosViewModel()
		{
			_httpClient = HttpClientFactory.Create("https://782a-37-130-115-34.ngrok-free.app");
		}

		public async Task AddOrUpdateProfilePhotosAsync(Stream imageStream)
		{
			if (imageStream != null)
			{
				try
				{
					var password = Preferences.Get("Password", string.Empty);

					string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{UserName}:{password}"));
					_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

					using var content = new MultipartFormDataContent();

					var streamContent = new StreamContent(imageStream);
					streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
					{
						Name = "profileImage",
						FileName = "profileImage.jpg"
					};
					streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
					content.Add(streamContent, "profileImage", "profileImage.jpg");

					string url = "https://782a-37-130-115-34.ngrok-free.app/UserUpdateApis/UpdateProfileImage";

					using (var response = await _httpClient.PostAsync(url, content))
					{
						if (response.IsSuccessStatusCode)
						{
							var responseContent = await response.Content.ReadAsStringAsync();
							await HandleResponseAsync(responseContent);
						}
						else
						{
							await Shell.Current.DisplayAlert("Hata", "Resim güncellenemedi.", "Tamam");
						}
					}
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
