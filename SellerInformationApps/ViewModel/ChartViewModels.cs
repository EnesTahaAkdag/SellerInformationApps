using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Maui.Dispatching;
using PraPazar.ServiceHelper;

namespace SellerInformationApps.ViewModel
{
	public partial class ChartPageViewModel : ObservableObject
	{
		[ObservableProperty]
		private ObservableCollection<SellerRaitingScore> data = new ObservableCollection<SellerRaitingScore>();

		public ChartPageViewModel()
		{
			Task.Run(GetCategoricalDataAsync);
		}

		public async Task GetCategoricalDataAsync()
		{
			try
			{
				var httpClient = HttpClientFactory.Create("https://70dd-37-130-115-34.ngrok-free.app");
				string url = "https://70dd-37-130-115-34.ngrok-free.app/SendDataToChart/ChartData";
				using (var request = new HttpRequestMessage(HttpMethod.Get, url))
				{
					using (var response = await httpClient.SendAsync(request))
					{
						if (response.IsSuccessStatusCode)
						{
							string json = await response.Content.ReadAsStringAsync();
							ApiResponses apiResponses = JsonConvert.DeserializeObject<ApiResponses>(json);
							if (apiResponses.Success)
							{
								var apiData = JsonConvert.DeserializeObject<List<SellerRaitingScore>>(apiResponses.Data.ToString());

								var ratingCounts = new Dictionary<int, int>();

								foreach (var item in apiData)
								{
									if (item.RatingScore.HasValue && item.RatingScore.Value != 0.00m)
									{
										int rating = (int)Math.Floor(item.RatingScore.Value);
										if (ratingCounts.ContainsKey(rating))
										{
											ratingCounts[rating]++;
										}
										else
										{
											ratingCounts[rating] = 1;
										}
									}
								}

								var result = new List<SellerRaitingScore>();
								for (int i = 1; i <= 5; i++)
								{
									int count = ratingCounts.ContainsKey(i) ? ratingCounts[i] : 0;
									result.Add(new SellerRaitingScore
									{
										StoreName = $"{i}",
										RatingScore = count
									});
								}

								MainThread.BeginInvokeOnMainThread(async () =>
								{
									foreach (var item in result)
									{
										Data.Add(new SellerRaitingScore { StoreName = "Boş", RatingScore = 2898 });
										Data.Add(new SellerRaitingScore { StoreName = "0", RatingScore = 2411 });
										Data.Add(item);
										
									}
								});
							}
							else
							{
								await MainThread.InvokeOnMainThreadAsync(async () =>
								{
									await App.Current.MainPage.DisplayAlert("HATA", $"API İsteği Başarısız: {apiResponses.ErrorMessage}", "Tamam");
								});
							}
						}
						else
						{
							await MainThread.InvokeOnMainThreadAsync(async () =>
							{
								await App.Current.MainPage.DisplayAlert("HATA", $"HTTP İsteği Başarısız: {response.StatusCode}", "Tamam");
							});
						}
					}
				}
			}
			catch (Exception ex)
			{
				await MainThread.InvokeOnMainThreadAsync(async () =>
				{
					await App.Current.MainPage.DisplayAlert("HATA", $"Hata Oluştu Apiye İstek Atılamadı: {ex.Message}", "Tamam");
				});
			}
		}

		public class SellerRaitingScore
		{
			public decimal? RatingScore { get; set; }
			public Color SegmentColor { get; set; }
			public string StoreName { get; set; }
		}

		public class ApiResponses
		{
			public bool Success { get; set; }
			public string ErrorMessage { get; set; }
			public object Data { get; set; }
			public int Count { get; set; }
			public int TotalCount { get; set; }
		}
	}
}
