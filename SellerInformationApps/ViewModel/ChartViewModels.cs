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
		private readonly AlertsHelper alertsHelper = new();

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

				if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
				{
					await alertsHelper.ShowSnackBar("Lütfen kullanıcı adı ve parola bilgilerini sağlayın.", true);
					return;
				}

				string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));
				var httpClient = HttpClientFactory.Create("https://1304-37-130-115-91.ngrok-free.app");

				string url = "https://1304-37-130-115-91.ngrok-free.app/ApplicationContentApi/ChartData";

				using (var request = new HttpRequestMessage(HttpMethod.Get, url))
				{
					request.Headers.TryAddWithoutValidation("Authorization", $"Basic {authHeaderValue}");
					using (var response = await httpClient.SendAsync(request))
					{
						if (!response.IsSuccessStatusCode)
						{
							await alertsHelper.ShowSnackBar($"HTTP İsteği Başarısız: {response.StatusCode}. Lütfen internet bağlantınızı kontrol edin.", true);
							return;
						}

						string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
						var apiResponses = JsonConvert.DeserializeObject<ApiResponses>(json);

						if (apiResponses?.Success == true)
						{
							var apiData = JsonConvert.DeserializeObject<List<SellerRatingScore>>(apiResponses.Data.ToString());

							if (apiData == null || !apiData.Any())
							{
								await alertsHelper.ShowSnackBar("Sunucudan geçerli veri alınamadı.", true);
								return;
							}

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
				}
			}
			catch (HttpRequestException httpEx)
			{
				await alertsHelper.ShowSnackBar($"Ağ hatası: {httpEx.Message}. Lütfen bağlantınızı kontrol edin.", true);
			}
			catch (JsonException jsonEx)
			{
				await alertsHelper.ShowSnackBar($"Yanıt işlenirken bir hata oluştu: {jsonEx.Message}", true);
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"API İsteği Başarısız: {ex.Message}", true);
			}
			finally
			{
				IsLoading = false;
			}
		}
	}
}
