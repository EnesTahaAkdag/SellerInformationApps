using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models.MarketPlaceSellerApp.ViewModel;
using ServiceHelper.Alerts;
using System.Collections.ObjectModel;
using System.Text;

namespace SellerInformationApps.ViewModel
{
	public partial class ChartPageViewModel : ObservableObject
	{
		private readonly AlertsHelper alertsHelper = new AlertsHelper();

		[ObservableProperty]
		private ObservableCollection<SellerRatingScore> data = new();

		[ObservableProperty]
		private bool isLoading;

		public ChartPageViewModel()
		{
			Task.Run(() => InitializeAsync());
		}


		private async Task InitializeAsync()
		{
			await GetCategoricalDataAsync();
		}

		public async Task GetCategoricalDataAsync()
		{
			try
			{
				IsLoading = true;

				var userName = Preferences.Get("UserName", string.Empty);
				var password = Preferences.Get("Password", string.Empty);

				string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));
				var httpClient = HttpClientFactory.Create("https://be65-37-130-115-91.ngrok-free.app");

				string url = "https://be65-37-130-115-91.ngrok-free.app/ApplicationContentApi/ChartData";

				using (var request = new HttpRequestMessage(HttpMethod.Get, url))
				{
					request.Headers.TryAddWithoutValidation("Authorization", $"Basic {authHeaderValue}");
					using (var response = await httpClient.SendAsync(request))
					{
						if (response.IsSuccessStatusCode)
						{
							string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
							var apiResponses = JsonConvert.DeserializeObject<ApiResponses>(json);

							if (apiResponses?.Success == true)
							{
								var apiData = JsonConvert.DeserializeObject<List<SellerRatingScore>>(apiResponses.Data.ToString());
								var ratingCounts = apiData
									.Where(item => item.RatingScore.HasValue && item.RatingScore.Value > 0)
									.GroupBy(item => (int)Math.Floor(item.RatingScore.Value))
									.ToDictionary(group => group.Key, group => group.Count());

								var result = Enumerable.Range(1, 5)
									.Select(i => new SellerRatingScore
									{
										StoreName = $"{i}",
										RatingScore = ratingCounts.TryGetValue(i, out var count) ? count : 0
									}).ToList();

								MainThread.BeginInvokeOnMainThread(() =>
								{
									Data.Clear();
									foreach (var item in result)
									{
										Data.Add(item);
									}
								});
							}
							else
							{
								await alertsHelper.ShowSnackBar(apiResponses?.ErrorMessage ?? "Bilinmeyen Hata", true)
									.ConfigureAwait(false);
							}
						}
						else
						{
							await alertsHelper.ShowSnackBar($"HTTP İsteği Başarısız: {response.StatusCode}", true)
								.ConfigureAwait(false);
						}
					}
				}
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"API İsteği Başarısız: {ex.Message}", true)
					.ConfigureAwait(false);
			}
			finally
			{
				IsLoading = false;
			}
		}
	}
}
