//using SellerInformationApps.Models;
//using SellerInformationApps.ViewModel;
//using PraPazar.ServiceHelper;
//using Newtonsoft.Json;
//using System.Text;
//namespace SellerInformationApps.Pages
//{
//	public partial class RegisterPage : ContentPage
//	{
//		public RegisterPage()
//		{
//			InitializeComponent();
//		}

//		public static async Task<bool> RegisterUser(UserViewModel user)
//		{
//			// Implement the method to register user to the database or API
//			// For example:
//			try
//			{
//				var httpClient = HttpClientFactory.Create("https://70dd-37-130-115-34.ngrok-free.app");
//				string url = "https://70dd-37-130-115-34.ngrok-free.app/RegisterAPI/RegisterUser";
//				var json = JsonConvert.SerializeObject(user);
//				var content = new StringContent(json, Encoding.UTF8, "application/json");

//				using (var response = await httpClient.PostAsync(url, content))
//				{
//					if (response.IsSuccessStatusCode)
//					{
//						return true;
//					}
//				}
//			}
//			catch (Exception ex)
//			{
//				await Application.Current.MainPage.DisplayAlert("Hata", $"Kullanýcý kaydedilemedi: {ex.Message}", "Tamam");
//			}

//			return false;
//		}

//		private async void OnRegisterButtonClicked(object sender, EventArgs e)
//		{
//			var viewModel = BindingContext as DataPickerViewModel;
//			if (viewModel != null)
//			{
//				viewModel.OnRegister();
//			}
//		}
//	}
//}
