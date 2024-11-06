using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using ServiceHelper.Alerts;
using ServiceHelper.Authentication;
using System.Text;

namespace SellerInformationApps.UpdatesViewModel
{
	public partial class SellerDetailsViewModel : Authentication
	{
		[ObservableProperty]
		private bool isLoading;

		private readonly AlertsHelper alertsHelper;

		public string StoreName { get; private set; }
		public string Telephone { get; private set; }
		public string Email { get; private set; }
		public string Address { get; private set; }
		public string Link { get; private set; }

		public SellerDetailsViewModel()
		{
			alertsHelper = new AlertsHelper();
		}

		public async Task LoadStoreDetailsAsync(long Id)
		{
			if (Id == 0) return;

			IsLoading = true;
			try
			{
				var userName = Preferences.Get("UserName", string.Empty);
				var password = Preferences.Get("Password", string.Empty);

				var httpClient = HttpClientFactory.Create("https://35ea-37-130-115-91.ngrok-free.app");

				string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));

				string url = $"https://35ea-37-130-115-91.ngrok-free.app/ApplicationContentApi/StoreDetails?Id={Id}";
				using (var request = new HttpRequestMessage(HttpMethod.Get, url))
				{
					request.Headers.TryAddWithoutValidation("Authorization", $"Basic { authHeaderValue}");
					using (var response = await httpClient.SendAsync(request))
					{
						if (response.IsSuccessStatusCode)
						{
							var json = await response.Content.ReadAsStringAsync();
							var storeDetails = JsonConvert.DeserializeObject<StoreDetailsApiResponse>(json);

							if (storeDetails?.Success == true && storeDetails.Data != null)
							{
								StoreName = storeDetails.Data.StoreName;
								Telephone = storeDetails.Data.Telephone;
								Email = storeDetails.Data.Email;
								Address = storeDetails.Data.Address;
								Link = storeDetails.Data.Link;
							}
							else
							{
								await alertsHelper.ShowSnackBar(storeDetails?.ErrorMessage ?? "Mağaza bilgisi alınamadı.", true);
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
				await alertsHelper.ShowSnackBar($"Bir hata oluştu: {ex.Message}", true);
			}
			finally
			{
				IsLoading = false;
			}
		}
	}
}
