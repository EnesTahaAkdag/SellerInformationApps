using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;

namespace SellerInformationApps.ViewModel
{
	public class SellerInfosViewModel
	{
		public ObservableCollection<StoreInfo> StoreInfos { get; private set; }

		public int CurrentPage { get; set; } = 1;

		private const int PageSize = 50;

		public SellerInfosViewModel()
		{
			StoreInfos = new ObservableCollection<StoreInfo>();
		}

		public async Task FetchDataFromAPIAsync()
		{
			try
			{
				var httpClient = HttpClientFactory.Create("https://0044-37-130-115-34.ngrok-free.app");
				string url = $"https://0044-37-130-115-34.ngrok-free.app/DataSendApp/MarketPlaceData?page={CurrentPage}&pageSize={PageSize}";

				using (var response = await httpClient.GetAsync(url))
				{
					if (response.IsSuccessStatusCode)
					{
						string json = await response.Content.ReadAsStringAsync();
						var apiResponse = JsonConvert.DeserializeObject<ApiResponsess>(json);
						if (apiResponse != null && apiResponse.Success)
						{
							foreach (var item in apiResponse.Data)
							{
								StoreInfos.Add(item);
							}
						}
						else
						{
							await ShowAlertAsync("HATA", $"API İsteği Başarısız: {apiResponse?.ErrorMessage}");
						}
					}
					else
					{
						await ShowAlertAsync("HATA", $"HTTP İsteği Başarısız: {response.StatusCode}");
					}
				}
			}
			catch (Exception ex)
			{
				await ShowAlertAsync("HATA", $"Hata Oluştu Apiye İstek Atılamadı: {ex.Message}");
			}
		}

		private async Task ShowAlertAsync(string title, string message)
		{
			await MainThread.InvokeOnMainThreadAsync(async () =>
			{
				await App.Current.MainPage.DisplayAlert(title, message, "Tamam");
			});
		}
	}
}
