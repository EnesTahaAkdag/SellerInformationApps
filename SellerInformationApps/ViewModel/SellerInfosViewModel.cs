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

		public IAsyncRelayCommand<long> OpenStoreDetailsCommand { get; }

		public SellerInfosViewModel()
		{
			StoreInfos = new ObservableCollection<StoreInfo>();
			OpenStoreDetailsCommand = new AsyncRelayCommand<long>(OpenStoreDetailsAsync);
			alertsHelper = new AlertsHelper();
		}


		[ObservableProperty]
		private bool isLoading;

		public ObservableCollection<StoreInfo> StoreInfos { get; private set; }

		public int CurrentPage { get; set; } = 1;

		public async Task FetchInitialDataAsync()
		{
			IsLoading = true;
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

				var httpClient = HttpClientFactory.Create("https://be65-37-130-115-91.ngrok-free.app");

				string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));
				string url = $"https://be65-37-130-115-91.ngrok-free.app/ApplicationContentApi/MarketPlaceData?page={CurrentPage}&pageSize={PageSize}";

				bool isRequestSuccessful = false;
				int retryCount = 3;

				while (!isRequestSuccessful && retryCount > 0)
				{
					try
					{
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
										isRequestSuccessful = true;
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
					catch (Exception ex)
					{
						retryCount--;
						if (retryCount == 0)
						{
							await alertsHelper.ShowSnackBar($"API isteği sırasında bir hata oluştu: {ex.Message}", true);
						}
					}
				}
			}
			finally
			{
				_semaphore.Release();
			}
		}

		private async Task OpenStoreDetailsAsync(long Id)
		{
			var storeDetailsViewModel = new SellerDetailsViewModel();
			await storeDetailsViewModel.LoadStoreDetailsAsync(Id);

			var popup = new StoreDetailsPopup(storeDetailsViewModel);
			await Application.Current.MainPage.ShowPopupAsync(popup);
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
