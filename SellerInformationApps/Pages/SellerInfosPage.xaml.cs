using System.Collections.ObjectModel;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;

namespace SellerInformationApps.Pages
{
	public partial class SellerInfosPage : ContentPage
	{
		public ObservableCollection<StoreInfo> StoreInfos { get; set; }

		private int currentPage = 1;

		private const int pageSize = 50;

		public SellerInfosPage()
		{
			InitializeComponent();
			StoreInfos = new ObservableCollection<StoreInfo>();
			BindingContext = this;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			await FetchDataFromAPI();
		}

		private async Task FetchDataFromAPI()
		{
			try
			{
				var httpClient = HttpClientFactory.Create("https://70dd-37-130-115-34.ngrok-free.app");
				string url = $"https://70dd-37-130-115-34.ngrok-free.app/DataSendApp/MarketPlaceData?page={currentPage}&pageSize{pageSize}";

				using (var request = new HttpRequestMessage(HttpMethod.Get, url))
				{
					using (var response = await httpClient.SendAsync(request))
					{
						if (response.IsSuccessStatusCode)
						{
							string json = await response.Content.ReadAsStringAsync();
							ApiResponsess apiResponse = JsonConvert.DeserializeObject<ApiResponsess>(json);
							if (apiResponse.Success)
							{
								foreach (var items in apiResponse.Data)
								{
									StoreInfos.Add(items);
								}
							}
							else
							{
								await MainThread.InvokeOnMainThreadAsync(async () =>
								{
									await DisplayAlert("HATA", $"API �ste�i Ba�ar�s�z:{apiResponse.ErrorMessage}", "Tamam");
								});
							}
						}
						else
						{
							await MainThread.InvokeOnMainThreadAsync(async () =>
							{
								await DisplayAlert("HATA", $"HTTP �ste�i Ba�ar�s�z:{response.StatusCode}", "Tamam");
							});
						}
					}
				}
			}
			catch (Exception ex)
			{
				await MainThread.InvokeOnMainThreadAsync(async () =>
				{
					await DisplayAlert("HATA", $"Hata Olu�tu Apiye �stek At�lamad�:{ex}", "Tamam");
				});
			}
		}

		private async void OnScrollViewScrolled(object sender, ScrolledEventArgs e)
		{
			var scrollView = sender as ScrollView;
			if (scrollView == null)
				return;

			double scrollingSpace = scrollView.ContentSize.Height - scrollView.Height;
			if (scrollingSpace <= e.ScrollY)
			{
				currentPage++;
				await FetchDataFromAPI();
			}
		}

		private void OnScrolled(object sender, ScrolledEventArgs e)
		{
			headerScroll.ScrollToAsync(e.ScrollX, 0, false);
		}
	}
}