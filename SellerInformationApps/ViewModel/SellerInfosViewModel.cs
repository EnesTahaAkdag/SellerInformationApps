using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using ServiceHelper.Alerts;
using ServiceHelper.Authentication;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Text;

namespace SellerInformationApps.ViewModel
{
	public partial class SellerInfosViewModel : Authentication
	{
		public AlertsHelper alertsHelper = new AlertsHelper();

		[ObservableProperty]
		private bool isLoading;

		public ObservableCollection<StoreInfo> StoreInfos { get; private set; }

		public int CurrentPage { get; set; } = 1;

		private const int PageSize = 50;

		private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);


		public SellerInfosViewModel(HttpClient httpClient)
		{
			StoreInfos = new ObservableCollection<StoreInfo>();
		}

		public async Task FetchDataOnScrollAsync()
		{
			if (IsLoading) return;
			await FetchDataFromAPIAsync();
		}

		public async Task FetchInitialDataAsync()
		{
			IsLoading = true;
			await FetchDataFromAPIAsync();
			IsLoading = false;
		}

		private async Task FetchDataFromAPIAsync()
		{
			await _semaphore.WaitAsync();
			try
			{
				var userName = Preferences.Get("UserName", string.Empty);
				var password = Preferences.Get("Password", string.Empty);

				var httpClient = HttpClientFactory.Create("https://de29-37-130-115-91.ngrok-free.app");
				string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

				string url = $"https://de29-37-130-115-91.ngrok-free.app/ApplicationContentApi/MarketPlaceData?page={CurrentPage}&pageSize={PageSize}";

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
							CurrentPage++;
						}
						else
						{
							await alertsHelper.ShowSnackBar("Veri alınamadı.", true);
						}
					}
					else
					{
						await alertsHelper.ShowSnackBar("API isteği başarısız oldu.",true);
					}
				}
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"API isteği sırasında bir hata oluştu: {ex.Message}",true);
			}
			finally
			{
				_semaphore.Release();
			}
		}
	}
}
