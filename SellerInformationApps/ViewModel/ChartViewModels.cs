using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;

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
				var httpClient = HttpClientFactory.Create("https://9d96-37-130-115-34.ngrok-free.app");
				string url = "https://9d96-37-130-115-34.ngrok-free.app/SendDataToChart/ChartData";

				var response = await httpClient.GetAsync(url);

				if (response.IsSuccessStatusCode)
				{
					string json = await response.Content.ReadAsStringAsync();
					var apiResponses = JsonConvert.DeserializeObject<ApiResponses>(json);
					if (apiResponses.Success)
					{
						var apiData = JsonConvert.DeserializeObject<List<SellerRaitingScore>>(apiResponses.Data.ToString());

						var ratingCounts = apiData
							.Where(item => item.RatingScore.HasValue && item.RatingScore.Value != 0.00m)
							.GroupBy(item => (int)Math.Floor(item.RatingScore.Value))
							.ToDictionary(g => g.Key, g => g.Count());

						var result = Enumerable.Range(1, 5).Select(i => new SellerRaitingScore
						{
							StoreName = $"{i}",
							RatingScore = ratingCounts.ContainsKey(i) ? ratingCounts[i] : 0
						}).ToList();

						MainThread.BeginInvokeOnMainThread(() =>
						{
							Data.Add(new SellerRaitingScore { StoreName = "Boş", RatingScore = 2898 });
							Data.Add(new SellerRaitingScore { StoreName = "0", RatingScore = 2411 });

							foreach (var item in result)
							{
								Data.Add(item);
							}
						});
					}
					else
					{
						await ShowAlertAsync("API İsteği Başarısız", apiResponses.ErrorMessage);
					}
				}
				else
				{
					await ShowAlertAsync("HTTP İsteği Başarısız", response.StatusCode.ToString());
				}
			}
			catch (Exception ex)
			{
				await ShowAlertAsync("Hata Oluştu", $"Apiye İstek Atılamadı: {ex.Message}");
			}
		}

		private async Task ShowAlertAsync(string title, string message)
		{
			await MainThread.InvokeOnMainThreadAsync(async () =>
			{
				await App.Current.MainPage.DisplayAlert("HATA", $"{title}: {message}", "Tamam");
			});
		}
	}
}
