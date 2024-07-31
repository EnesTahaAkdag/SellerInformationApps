using System.Collections.ObjectModel;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;


namespace SellerInformationApps.Pages;

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
			var httpClient = HttpClientFactory.Create("https://4309-37-130-115-34.ngrok-free.app");
			string url = $"https://4309-37-130-115-34.ngrok-free.app/DataSendApp/MarketPlaceData?page={currentPage}&pageSize{pageSize}";

			using (var request = new HttpRequestMessage(HttpMethod.Get, url))
			{
				using (var response = await httpClient.SendAsync(request))
				{
					if (response.IsSuccessStatusCode)
					{
						string json = await response.Content.ReadAsStringAsync();
						ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(json);
						if (apiResponse.Success)
						{
							foreach (var item in apiResponse.Data)
							{
								StoreInfos.Add(item);
							}
						}
						else
						{
							await MainThread.InvokeOnMainThreadAsync(async () =>
							{
								await DisplayAlert("HATA", $"API Ýsteði Baþarýsýz:{apiResponse.ErrorMessage}", "Tamam");
							});
						}
					}
					else
					{
						await MainThread.InvokeOnMainThreadAsync(async () =>
						{
							await DisplayAlert("HATA", $"HTTP Ýsteði Baþarýsýz:{response.StatusCode}", "Tamam");
						});
					}
				}
			}
		}
		catch (Exception ex)
		{
			await MainThread.InvokeOnMainThreadAsync(async () =>
			{
				await DisplayAlert("HATA", $"Hata Oluþtu Apiye Ýstek Atýlamadý:{ex}", "Tamam");
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
	public class StoreInfo
	{
		public long Id { get; set; }
		public string Link { get; set; }
		public string StoreName { get; set; }
		public string Telephone { get; set; }
		public string Email { get; set; }
		public string Address { get; set; }
		public string Fax { get; set; }
		public string Mersis { get; set; }
		public string Category { get; set; }
		public decimal? StoreScore { get; set; }
		public int? NumberOfRatings { get; set; }
		public int? NumberOfFollowers { get; set; }
		public string AverageDeliveryTime { get; set; }
		public string ResponseTime { get; set; }
		public decimal? RatingScore { get; set; }
		public int? NumberOfComments { get; set; }
		public string NumberOfProducts { get; set; }
		public string SellerName { get; set; }
		public string VKN { get; set; }
	}
	public class ApiResponse
	{
		public bool Success { get; set; }
		public string ErrorMessage { get; set; }
		public List<StoreInfo> Data { get; set; }
		public int Page { get; set; }
		public int PageSize { get; set; }
		public int TotalCount { get; set; }
		public int TotalPage { get; set; }
	}
}