using System.Collections.ObjectModel;
using Newtonsoft.Json;
using SellerInformationApps.Models;
using PraPazar.ServiceHelper;

namespace SellerInformationApps.Pages
{
	public partial class UserListPage : ContentPage
	{
		public ObservableCollection<UserList> UserLists { get; set; }

		public UserListPage()
		{
			InitializeComponent();
			UserLists = new ObservableCollection<UserList>();
			BindingContext = this;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			await UserListDataFromAPI();
		}

		public async Task UserListDataFromAPI()
		{
			try
			{
				var httpClient = HttpClientFactory.Create("https://70dd-37-130-115-34.ngrok-free.app");
				string url = "https://70dd-37-130-115-34.ngrok-free.app/UserListAPI/UserList";
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
								await DisplayAlert("HATA", $"API Ýsteði Baþarýsýz: {apiResponse.ErrorMessage}", "Tamam");
							}
						}
						else
						{
							await DisplayAlert("HATA", $"HTTP Ýsteði Baþarýsýz: {response.StatusCode}", "Tamam");
						}
					}
				}
			}
			catch (Exception ex)
			{
				await DisplayAlert("HATA", $"Hata Oluþtu Apiye Ýstek Atýlamadý: {ex.Message}", "Tamam");
			}
		}

		private void OnScrolled(object sender, ScrolledEventArgs e)
		{
			headerScroll.ScrollToAsync(e.ScrollX, 0, false);
		}
	}
}
