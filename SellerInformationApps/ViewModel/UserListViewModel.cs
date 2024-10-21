using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using ServiceHelper.Alerts;
using ServiceHelper.Authentication;
using System.Collections.ObjectModel;
using System.Text;

namespace SellerInformationApps.ViewModel
{
	public partial class UserListViewModel : Authentication
	{
		public AlertsHelper alertsHelper = new AlertsHelper();


		[ObservableProperty]
		private bool isLoading;

		public ObservableCollection<UserList> UserLists { get; set; } = new ObservableCollection<UserList>();

		public async Task UserListDataFromAPI()
		{
			try
			{
				IsLoading = true;

				var userName = Preferences.Get("UserName", string.Empty);
				var password = Preferences.Get("Password", string.Empty);

				string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));

				var httpClient = HttpClientFactory.Create("https://de29-37-130-115-91.ngrok-free.app");
				httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeaderValue);

				string url = "https://de29-37-130-115-91.ngrok-free.app/UserDataSendApi/UserList";
				using (var request = new HttpRequestMessage(HttpMethod.Get, url))
				{
					using (var response = await httpClient.SendAsync(request))
					{
						if (response.IsSuccessStatusCode)
						{
							string json = await response.Content.ReadAsStringAsync();
							var apiResponse = JsonConvert.DeserializeObject<UserApiResponse>(json);
							if (apiResponse.Success)
							{
								foreach (var item in apiResponse.Data)
								{
									UserLists.Add(item);
								}
							}
							else
							{
								await alertsHelper.ShowSnackBar($"API İsteği Başarısız: {apiResponse.ErrorMessage}", true);
							}
						}
						else
						{
							await alertsHelper.ShowSnackBar($"HTTP İsteği Başarısız: {response.StatusCode}", true);
						}
					}
				}
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"Hata Oluştu Apiye İstek Atılamadı: {ex.Message}", true);
			}
			finally
			{
				IsLoading = false;
			}
		}
	}
}
