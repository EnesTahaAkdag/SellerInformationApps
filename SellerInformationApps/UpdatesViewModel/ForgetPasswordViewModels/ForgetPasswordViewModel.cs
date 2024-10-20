﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SellerInformationApps.Models;
using System.Text;
using Newtonsoft.Json;
using CommunityToolkit.Maui.Views;
using SellerInformationApps.PopUps.ForgetPasswordPopUps;
using PraPazar.ServiceHelper;
using ServiceHelper.Alerts;

namespace SellerInformationApps.UpdatesViewModel.ForgetPasswordViewModels
{
	public partial class ForgetPasswordViewModel : ObservableObject
	{
		public AlertsHelper alertsHelper = new AlertsHelper();

		private readonly Popup _popup;

		[ObservableProperty]
		private string userName;

		public IRelayCommand SubmitCommand { get; }
		public IRelayCommand CancelCommand { get; }

		public ForgetPasswordViewModel(Popup popup)
		{
			_popup = popup;
			SubmitCommand = new AsyncRelayCommand(SubmitCommandAsync);
			CancelCommand = new RelayCommand(ClosePopup);
		}

		private void ClosePopup()
		{
			_popup?.Close();
		}

		[RelayCommand]
		public async Task SubmitCommandAsync()
		{
			if (!IsFormValid())
			{
				await alertsHelper.ShowSnackBar("Lütfen tüm alanları doldurduğunuzdan emin olun",true);
				return;
			}

			try
			{
				var forgetPassword = CreateForgetPassword();

				if (forgetPassword == null)
				{
					await alertsHelper.ShowSnackBar("Bir hata oluştu", true);
					return;
				}

				var httpClient = HttpClientFactory.Create("https://59b7-37-130-115-91.ngrok-free.app");
				string url = "https://59b7-37-130-115-91.ngrok-free.app/RegisterAndLoginApi/ForgetPassword";

				var content = new StringContent(JsonConvert.SerializeObject(forgetPassword), Encoding.UTF8, "application/json");

				using (var response = await httpClient.PostAsync(url, content))
				{
					if (response.IsSuccessStatusCode)
					{
						string json = await response.Content.ReadAsStringAsync();
						var apiResponse = JsonConvert.DeserializeObject<ForgetPasswordApiResponse>(json);

						if (apiResponse.Success)
						{
							Preferences.Set("UserName",UserName);
							await alertsHelper.ShowSnackBar("Doğrulama kodu e-posta adresinize gönderildi");

							var verificationCodeEntryPopup = new VerificationCodeEntryPopup();

							Application.Current.MainPage.ShowPopup(verificationCodeEntryPopup);
							ClosePopup(); 

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
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"Bir hata oluştu: {ex.Message}", true);
			}
		}

		private bool IsFormValid()
		{
			return !string.IsNullOrWhiteSpace(UserName);
		}

		private ForgetPassword CreateForgetPassword()
		{
			return new ForgetPassword
			{
				UserName = UserName,
			};
		}
	}
}
