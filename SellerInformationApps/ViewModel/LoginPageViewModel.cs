﻿using CommunityToolkit.Maui.Views;
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
		private readonly AlertsHelper alertsHelper = new();
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
					await alertsHelper.ShowSnackBar("Giriş bilgileri oluşturulurken bir hata oluştu.", true);
					return;
				}

				var httpClient = HttpClientFactory.Create("https://1304-37-130-115-91.ngrok-free.app");
				string url = "https://1304-37-130-115-91.ngrok-free.app/RegisterAndLoginApi/LoginUserData";
				var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

				using (var request = new HttpRequestMessage(HttpMethod.Post, url))
				{
					request.Content = content;

					using (var response = await httpClient.SendAsync(request))
					{
						if (!response.IsSuccessStatusCode)
						{
							await alertsHelper.ShowSnackBar($"HTTP isteği başarısız oldu: {response.StatusCode}. Lütfen internet bağlantınızı kontrol edin.", true);
							return;
						}

						string json = await response.Content.ReadAsStringAsync();
						var apiResponse = JsonConvert.DeserializeObject<LoginApiResponse>(json);

						if (apiResponse == null)
						{
							await alertsHelper.ShowSnackBar("Sunucudan alınan yanıt işlenemedi. Lütfen daha sonra tekrar deneyin.", true);
							return;
						}

						if (apiResponse.Success)
						{
							Preferences.Set("UserName", UserName);
							Preferences.Set("Password", Password);
							authentication.LogIn();
							await Shell.Current.GoToAsync("//MainPage");
						}
						else
						{
							await alertsHelper.ShowSnackBar(apiResponse.ErrorMessage ?? "Giriş başarısız oldu.", true);
						}
					}
				}
			}
			catch (HttpRequestException httpEx)
			{
				await alertsHelper.ShowSnackBar($"Ağ hatası: {httpEx.Message}. Lütfen bağlantınızı kontrol edin.", true);
			}
			catch (JsonException jsonEx)
			{
				await alertsHelper.ShowSnackBar($"Yanıt işlenirken bir hata oluştu: {jsonEx.Message}", true);
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"Beklenmeyen bir hata oluştu: {ex.Message}", true);
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

		private async Task NavigateToRegisterPageAsync()
		{
			await Shell.Current.GoToAsync("//RegisterPage");
		}

		private async Task RememberPasswordAsync()
		{
			var popup = new ForgetPasswordPopUp();
			Shell.Current.ShowPopup(popup);
			await Task.CompletedTask;
		}
	}
}
