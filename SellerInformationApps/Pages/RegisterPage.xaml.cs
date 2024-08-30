using SellerInformationApps.Models;
using SellerInformationApps.ViewModel;
using PraPazar.ServiceHelper;
using Newtonsoft.Json;
using System.Text;
namespace SellerInformationApps.Pages
{
	public partial class RegisterPage : ContentPage
	{
		public RegisterPage()
		{
			InitializeComponent();
		}

		public async void RegisterUser(UserViewModel user)
		{
			try
			{
				var httpClient = HttpClientFactory.Create("https://70dd-37-130-115-34.ngrok-free.app");
				string url = "https://70dd-37-130-115-34.ngrok-free.app/RegisterAPI/RegisterUser";
				var json = JsonConvert.SerializeObject(user);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				using (var response = await httpClient.PostAsync(url, content))
				{
					if (response.IsSuccessStatusCode)
					{
						await Shell.Current.GoToAsync("//LoginPage");
					}
				}
			}
			catch (Exception ex)
			{
				await Application.Current.MainPage.DisplayAlert("Hata", $"Kullanýcý kaydedilemedi: {ex.Message}", "Tamam");
			}
		}
	}
}
