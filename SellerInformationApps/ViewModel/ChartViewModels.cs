using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using System.Text;
using System.Net.Http.Headers;

namespace SellerInformationApps.ViewModel
{
	public partial class ChartPageViewModel : ObservableObject
	{
		[ObservableProperty]
		private ObservableCollection<SellerRaitingScore> data = new ObservableCollection<SellerRaitingScore>();

		[ObservableProperty]
		private bool isLoading;

		public ChartPageViewModel()
		{
			Task.Run(GetCategoricalDataAsync);
		}

		public async Task GetCategoricalDataAsync()
		{
			try
			{
				IsLoading = true;

				var userName = Preferences.Get("UserName", string.Empty);
				var password = Preferences.Get("Password", string.Empty);

				string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));

				var httpClient = HttpClientFactory.Create("https://4b42-37-130-115-34.ngrok-free.app");
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",authHeaderValue);

				string url = "https://4b42-37-130-115-34.ngrok-free.app/SendDataToChart/ChartData";

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
			finally { IsLoading = false; }
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
