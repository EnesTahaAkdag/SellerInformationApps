using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using ServiceHelper.Alerts;
using ServiceHelper.Authentication;
using System.Text;

namespace SellerInformationApps.UpdatesViewModel
{
	public partial class SellerDetailsViewModel : Authentication
	{
		[ObservableProperty]
		private bool isLoading;

		private readonly AlertsHelper alertsHelper;

		[ObservableProperty]
		private string storeName;

		[ObservableProperty]
		private string telephone;

		[ObservableProperty]
		private string email;

		[ObservableProperty]
		private string address;

		[ObservableProperty]
		private string link;

		public SellerDetailsViewModel()
		{
			alertsHelper = new AlertsHelper();
		}

		public async Task LoadStoreDetailsAsync(long id)
		{
			if (id <= 0)
			{
				await alertsHelper.ShowSnackBar("Geçersiz mağaza kimliği.", true);
				return;
			}

			IsLoading = true;
			try
			{
				var userName = Preferences.Get("UserName", string.Empty);
				var password = Preferences.Get("Password", string.Empty);

				if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
				{
					await alertsHelper.ShowSnackBar("Kullanıcı adı veya şifre eksik. Lütfen giriş yapın.", true);
					return;
				}

				var httpClient = HttpClientFactory.Create("https://1304-37-130-115-91.ngrok-free.app");
				string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));

				string url = $"https://1304-37-130-115-91.ngrok-free.app/ApplicationContentApi/StoreDetails?Id={id}";
				using (var request = new HttpRequestMessage(HttpMethod.Get, url))
				{
					request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeaderValue);
					using (var response = await httpClient.SendAsync(request))
					{
						if (response.IsSuccessStatusCode)
						{
							var json = await response.Content.ReadAsStringAsync();
							var storeDetails = JsonConvert.DeserializeObject<StoreDetailsApiResponse>(json);

							if (storeDetails?.Success == true && storeDetails.Data != null)
							{
								StoreName = storeDetails.Data.StoreName;
								Telephone = storeDetails.Data.Telephone;
								Email = storeDetails.Data.Email;
								Address = storeDetails.Data.Address;
								Link = storeDetails.Data.Link;
							}
							else
							{
								await alertsHelper.ShowSnackBar(storeDetails?.ErrorMessage ?? "Mağaza bilgisi alınamadı.", true);
							}
						}
						else
						{
							await alertsHelper.ShowSnackBar($"API isteği başarısız oldu: {response.StatusCode}", true);
						}
					}
				}
			}
			catch (HttpRequestException httpEx)
			{
				await alertsHelper.ShowSnackBar($"Ağ bağlantısında hata oluştu: {httpEx.Message}", true);
			}
			catch (JsonException jsonEx)
			{
				await alertsHelper.ShowSnackBar($"Veri işleme hatası: {jsonEx.Message}", true);
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"Bir hata oluştu: {ex.Message}", true);
			}
			finally
			{
				IsLoading = false;
			}
		}
	}
}
