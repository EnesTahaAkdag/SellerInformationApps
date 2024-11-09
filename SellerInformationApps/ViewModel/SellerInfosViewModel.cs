using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using SellerInformationApps.PopUps;
using SellerInformationApps.UpdatesViewModel;
using ServiceHelper.Alerts;
using ServiceHelper.Authentication;
using System.Collections.ObjectModel;
using System.Text;

namespace SellerInformationApps.ViewModel
{
	public partial class SellerInfosViewModel : Authentication
	{
		private const int PageSize = 50;
		private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
		private readonly AlertsHelper alertsHelper;

		[ObservableProperty]
		private bool isProcessing;

		[ObservableProperty]
		private bool isLoading;

		public ObservableCollection<StoreInfo> StoreInfos { get; private set; }

		public int CurrentPage { get; set; } = 1;

		public IAsyncRelayCommand<long> OpenDetailsCommand { get; }

		public SellerInfosViewModel()
		{
			StoreInfos = new ObservableCollection<StoreInfo>();
			OpenDetailsCommand = new AsyncRelayCommand<long>(async (id) => await OpenStoreDetailsAsync(id));
			alertsHelper = new AlertsHelper();
		}

		public async Task FetchInitialDataAsync()
		{
			IsLoading = true;
			StoreInfos.Clear();
			CurrentPage = 1;
			await FetchDataFromAPIAsync();
			IsLoading = false;
		}

		public async Task FetchDataOnScrollAsync()
		{
			if (IsLoading) return;
			await FetchDataFromAPIAsync();
		}

		private async Task FetchDataFromAPIAsync()
		{
			await _semaphore.WaitAsync();
			try
			{
				var userName = Preferences.Get("UserName", string.Empty);
				var password = Preferences.Get("Password", string.Empty);

				var httpClient = HttpClientFactory.Create("https://1304-37-130-115-91.ngrok-free.app");

				string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));
				string url = $"https://1304-37-130-115-91.ngrok-free.app/ApplicationContentApi/MarketPlaceData?page={CurrentPage}&pageSize={PageSize}";

				using (var request = new HttpRequestMessage(HttpMethod.Get, url))
				{
					request.Headers.TryAddWithoutValidation("Authorization", $"Basic {authHeaderValue}");
					using (var response = await httpClient.SendAsync(request))
					{
						if (response.IsSuccessStatusCode)
						{
							string json = await response.Content.ReadAsStringAsync();
							var apiResponse = JsonConvert.DeserializeObject<ApiResponsess>(json);

							if (apiResponse != null && apiResponse.Success)
							{
								foreach (var item in apiResponse.Data)
								{
									TrimAllStringProperties(item, 16);
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
							await alertsHelper.ShowSnackBar("API isteği başarısız oldu.", true);
						}
					}
				}
			}
			finally
			{
				_semaphore.Release();
			}
		}

		private async Task OpenStoreDetailsAsync(long id)
		{
			IsProcessing = true;

			foreach (var store in StoreInfos)
			{
				store.IsSelected = false;
			}

			var selectedStore = StoreInfos.FirstOrDefault(s => s.Id == id);
			if (selectedStore != null)
			{
				selectedStore.IsSelected = true;
			}

			var storeDetailsViewModel = new SellerDetailsViewModel();
			await storeDetailsViewModel.LoadStoreDetailsAsync(id);

			var popup = new StoreDetailsPopup(storeDetailsViewModel);
			await Application.Current.MainPage.ShowPopupAsync(popup);

			IsProcessing = false;
		}

		private void TrimAllStringProperties(StoreInfo item, int maxLength)
		{
			var properties = typeof(StoreInfo).GetProperties()
				.Where(prop => prop.PropertyType == typeof(string) && prop.CanWrite && prop.CanRead);

			foreach (var prop in properties)
			{
				var value = (string)prop.GetValue(item);
				if (!string.IsNullOrEmpty(value) && value.Length > maxLength)
				{
					prop.SetValue(item, value.Substring(0, maxLength) + "...");
				}
			}
		}
	}
}
