using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using ServiceHelper.Authentication;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SellerInformationApps.ViewModel
{
	public partial class SellerInfosViewModel : Authentication
	{
		[ObservableProperty]
		private bool isLoading;

		public ObservableCollection<StoreInfo> StoreInfos { get; private set; }

		public int CurrentPage { get; set; } = 1;

		private const int PageSize = 50;

		private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

		private readonly HttpClient _httpClient;

		public SellerInfosViewModel(HttpClient httpClient)
		{
			_httpClient = httpClient;
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

				string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

				string url = $"https://8b27-37-130-115-34.ngrok-free.app/DataSendApp/MarketPlaceData?page={CurrentPage}&pageSize={PageSize}";

				using (var response = await _httpClient.GetAsync(url))
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
							await ShowAlertAsync("Hata", "Veri alınamadı.");
						}
					}
					else
					{
						await ShowAlertAsync("Hata", "API isteği başarısız oldu.");
					}
				}
			}
			catch (Exception ex)
			{
				await ShowAlertAsync("Hata", $"API isteği sırasında bir hata oluştu: {ex.Message}");
			}
			finally
			{
				_semaphore.Release();
			}
		}

		private async Task ShowAlertAsync(string title, string message)
		{
			await MainThread.InvokeOnMainThreadAsync(() =>
				App.Current.MainPage.DisplayAlert(title, message, "Tamam"));
		}
	}
}
