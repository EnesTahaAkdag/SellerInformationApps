using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellerInformationApps.ViewModel
{
	public partial class UserListViewModel
	{
		public ObservableCollection<UserList> UserLists { get; set; } = new ObservableCollection<UserList>();

		public async Task UserListDataFromAPI()
		{
			try
			{
				var httpClient = HttpClientFactory.Create("https://f038-37-130-115-34.ngrok-free.app");
				string url = "https://f038-37-130-115-34.ngrok-free.app/UserListAPI/UserList";
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
		}
	}
}
