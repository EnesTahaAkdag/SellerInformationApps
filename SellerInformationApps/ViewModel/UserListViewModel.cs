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
		private readonly AlertsHelper alertsHelper = new AlertsHelper();

		[ObservableProperty]
		private bool isLoading;

		public ObservableCollection<UserList> UserLists { get; } = new ObservableCollection<UserList>();

		public int CurrentPage { get; set; } = 1;

		public const int PageSize = 50;

		public async Task FetchDataOnScrollAsync()
		{
			if (IsLoading) return;

			IsLoading = true;
			await UserListDataFromAPI();
			IsLoading = false;
		}

		public async Task FetchInitialDataAsync()
		{
			IsLoading = true;
			UserLists.Clear();
			CurrentPage = 1;
			await UserListDataFromAPI();
			IsLoading = false;
		}

		private async Task UserListDataFromAPI()
		{
			try
			{
				var userName = Preferences.Get("UserName", string.Empty);
				var password = Preferences.Get("Password", string.Empty);

				var httpClient = HttpClientFactory.Create("https://be65-37-130-115-91.ngrok-free.app");

				string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));
				string url = $"https://be65-37-130-115-91.ngrok-free.app/UserDataSendApi/UserList?page={CurrentPage}&pageSize={PageSize}";

				using (var request = new HttpRequestMessage(HttpMethod.Get, url))
				{
					request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeaderValue);
					using (var response = await httpClient.SendAsync(request))
					{
						if (response.IsSuccessStatusCode)
						{
							string json = await response.Content.ReadAsStringAsync();
							var apiResponse = JsonConvert.DeserializeObject<UserApiResponse>(json);
							if (apiResponse != null && apiResponse.Success)
							{
								foreach (var item in apiResponse.Data)
								{
									UserLists.Add(item);
								}
								CurrentPage++;
							}
							else
							{
								await alertsHelper.ShowSnackBar($"API İsteği Başarısız: {apiResponse?.ErrorMessage}", true);
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
		}
	}
}
