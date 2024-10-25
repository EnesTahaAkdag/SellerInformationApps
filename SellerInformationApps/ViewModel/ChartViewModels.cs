using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using ServiceHelper.Alerts;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Text;

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
			var alertsHelper = new AlertsHelper();
			try
			{
				IsLoading = true;

				var userName = Preferences.Get("UserName", string.Empty);
				var password = Preferences.Get("Password", string.Empty);

				var httpClient = HttpClientFactory.Create("https://48d6-37-130-115-91.ngrok-free.app");
				string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

				string url = "/ApplicationContentApi/ChartData";
				var response = await httpClient.GetAsync(url);

				if (response.IsSuccessStatusCode)
				{
					string json = await response.Content.ReadAsStringAsync();
					var apiResponses = JsonConvert.DeserializeObject<ApiResponses>(json);
					if (apiResponses.Success)
					{
						var apiData = JsonConvert.DeserializeObject<List<SellerRaitingScore>>(apiResponses.Data.ToString());

						var ratingCounts = new Dictionary<int, int>();

						foreach (var item in apiData.Where(item => item.RatingScore.HasValue && item.RatingScore.Value != 0.00m))
						{
							int scoreKey = (int)Math.Floor(item.RatingScore.Value);
							if (ratingCounts.ContainsKey(scoreKey))
							{
								ratingCounts[scoreKey]++;
							}
							else
							{
								ratingCounts[scoreKey] = 1;
							}
						}

						var result = Enumerable.Range(1, 5).Select(i => new SellerRaitingScore
						{
							StoreName = $"{i}",
							RatingScore = ratingCounts.ContainsKey(i) ? ratingCounts[i] : 0
						}).ToList();

						MainThread.BeginInvokeOnMainThread(() =>
						{
							Data.Clear();
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
						await alertsHelper.ShowSnackBar(apiResponses.ErrorMessage, true);
					}
				}
				else
				{
					await alertsHelper.ShowSnackBar($"HTTP İsteği Başarısız {response.StatusCode.ToString()}", true);
				}
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"Apiye İstek Atılamadı: {ex.Message}", true);
			}
			finally
			{
				IsLoading = false;
			}
		}
	}
}
