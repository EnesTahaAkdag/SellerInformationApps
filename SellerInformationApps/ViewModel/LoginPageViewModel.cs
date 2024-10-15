using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using SellerInformationApps.PopUps.ForgetPasswordPopUps;
using ServiceHelper.Alerts;
using ServiceHelper.Authentication;
using System.Text;
using System.Windows.Input;

namespace SellerInformationApps.ViewModel
{
	public partial class LoginPageViewModel : ObservableObject
	{
		public AlertsHelper alertsHelper = new AlertsHelper();
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
				await alertsHelper.ShowSnackBar("Lütfen tüm alanları doldurduğunuzdan emin olun.", true);
				return;
			}

			try
			{
				var user = CreateLoginUser();
				if (user == null)
				{
					await alertsHelper.ShowSnackBar("Giriş yaparken bir hata oluştu.", true);
					return;
				}

				var httpClient = HttpClientFactory.Create("https://a8c0-37-130-115-91.ngrok-free.app");
				string url = "https://a8c0-37-130-115-91.ngrok-free.app/RegisterAndLoginApi/LoginUserData";
				var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

				using (var response = await httpClient.PostAsync(url, content))
				{
					await HandleResponseAsync(response);
				}
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"Hata oluştu: {ex.Message}", true);
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
					await alertsHelper.ShowSnackBar(apiResponse.ErrorMessage, true);
				}
			}
			else
			{
				await alertsHelper.ShowSnackBar($"HTTP isteği başarısız oldu: {response.StatusCode}", true);
			}
		}

		private async Task NavigateToRegisterPageAsync()
		{
			await Shell.Current.GoToAsync("//RegisterPage");
		}
		private async Task RememberPasswordAsync()
		{
			var popup = new ForgetPasswordPopUp();
			Application.Current.MainPage.ShowPopup(popup);
			await Task.CompletedTask;
		}
	}
}
