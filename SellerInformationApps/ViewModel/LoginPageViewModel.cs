using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using SellerInformationApps.PopUps;
using ServiceHelper.Authentication;
using System.Text;
using System.Windows.Input;

namespace SellerInformationApps.ViewModel
{
	public partial class LoginPageViewModel : ObservableObject
	{
		private readonly Authentication authentication;

		[ObservableProperty]
		private string userName;

		[ObservableProperty]
		private string password;

		public ICommand RegisterCommand { get; }
		public IRelayCommand RememberYourPassword { get; }


		public LoginPageViewModel()
		{
			authentication = Authentication.Instance;
			RegisterCommand = new AsyncRelayCommand(NavigateToRegisterPageAsync);
			RememberYourPassword = new AsyncRelayCommand(RememberPasswordAsync);

		}

		[RelayCommand]
		public async Task LoginAsync()
		{
			if (!IsFormValid())
			{
				await App.Current.MainPage.DisplayAlert("Hata", "Lütfen tüm alanları doldurduğunuzdan emin olun.", "Tamam");
				return;
			}

			try
			{
				var user = CreateLoginUser();
				if (user == null)
				{
					await App.Current.MainPage.DisplayAlert("Hata", "Giriş yaparken bir hata oluştu.", "Tamam");
					return;
				}

				var httpClient = HttpClientFactory.Create("https://b8ac-37-130-115-91.ngrok-free.app/");
				string url = "https://b8ac-37-130-115-91.ngrok-free.app/RegisterAndLoginApi/LoginUserData";
				var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

				using (var response = await httpClient.PostAsync(url, content))
				{
					await HandleResponseAsync(response);
				}
			}
			catch (Exception ex)
			{
				await App.Current.MainPage.DisplayAlert("Hata", $"Hata oluştu: {ex.Message}", "Tamam");
			}
		}

		private bool IsFormValid()
		{
			return !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password);
		}

		private LoginUser CreateLoginUser()
		{
			return new LoginUser
			{
				UserName = UserName?.Trim(),
				Password = Password?.Trim()
			};
		}

		private async Task HandleResponseAsync(HttpResponseMessage response)
		{
			if (response.IsSuccessStatusCode)
			{
				string json = await response.Content.ReadAsStringAsync();
				var apiResponse = JsonConvert.DeserializeObject<LoginApiResponse>(json);

				if (apiResponse.Success)
				{
					Preferences.Set("UserName", UserName);
					Preferences.Set("Password", Password);
					authentication.LogIn();
					await Shell.Current.GoToAsync("//MainPage");
				}
				else
				{
					await App.Current.MainPage.DisplayAlert("Başarısız", apiResponse.ErrorMessage, "Tamam");
				}
			}
			else
			{
				await App.Current.MainPage.DisplayAlert("Hata", $"HTTP isteği başarısız oldu: {response.StatusCode}", "Tamam");
			}
		}

		private async Task NavigateToRegisterPageAsync()
		{
			await Shell.Current.GoToAsync("//RegisterPage");
		}
		private static Task RememberPasswordAsync()
		{
			var popup = new RememberPasswordPupUp();
			Application.Current.MainPage.ShowPopup(popup);
			return Task.CompletedTask;
		}
	}
}
