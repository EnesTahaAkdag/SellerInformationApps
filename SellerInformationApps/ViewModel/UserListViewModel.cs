using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using ServiceHelper.Authentication;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellerInformationApps.ViewModel
{
	public partial class UserListViewModel : Authentication
	{
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
				

				var httpClient = HttpClientFactory.Create("https://314b-37-130-115-91.ngrok-free.app/");
				httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeaderValue);

				string url = "https://314b-37-130-115-91.ngrok-free.app/UserDataSendApi/UserList";
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
								await App.Current.MainPage.DisplayAlert("HATA", $"API İsteği Başarısız: {apiResponse.ErrorMessage}", "Tamam");
							}
						}
						else
						{
							await App.Current.MainPage.DisplayAlert("HATA", $"HTTP İsteği Başarısız: {response.StatusCode}", "Tamam");
						}
					}
				}
			}
			catch (Exception ex)
			{
				await App.Current.MainPage.DisplayAlert("HATA", $"Hata Oluştu Apiye İstek Atılamadı: {ex.Message}", "Tamam");
			}
			finally { IsLoading = false; }
		}
	}
}
