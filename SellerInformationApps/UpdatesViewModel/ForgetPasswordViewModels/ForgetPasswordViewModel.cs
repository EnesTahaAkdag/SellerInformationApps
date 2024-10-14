using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SellerInformationApps.Models;
using System.Text;
using Newtonsoft.Json;
using CommunityToolkit.Maui.Views;
using SellerInformationApps.PopUps.ForgetPasswordPopUps;
using PraPazar.ServiceHelper;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Alerts;

namespace SellerInformationApps.UpdatesViewModel.ForgetPasswordViewModels
{
	public partial class ForgetPasswordViewModel : ObservableObject
	{
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
				await ShowToast("Lütfen tüm alanları doldurduğunuzdan emin olun");
				return;
			}

			try
			{
				var forgetPassword = CreateForgetPassword();

				if (forgetPassword == null)
				{
					await ShowToast("Bir hata oluştu");
					return;
				}

				var httpClient = HttpClientFactory.Create("https://c846-37-130-115-91.ngrok-free.app");
				string url = "https://c846-37-130-115-91.ngrok-free.app/RegisterAndLoginApi/ForgetPassword";

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
							await Shell.Current.DisplayAlert("Başarılı", "Doğrulama kodu e-posta adresinize gönderildi", "Tamam");

							var verificationCodeEntryPopup = new VerificationCodeEntryPopup();

							Application.Current.MainPage.ShowPopup(verificationCodeEntryPopup);
							ClosePopup(); 

						}

						else
						{
							await ShowToast(apiResponse.ErrorMessage);
						}
					}
					else
					{
						await ShowToast($"HTTP isteği başarısız oldu: {response.StatusCode}");
					}
				}
			}
			catch (Exception ex)
			{
				await ShowToast($"Bir hata oluştu: {ex.Message}");
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

		private async Task ShowToast(string message, bool isSuccess = false)
		{
			var toast = Toast.Make(message, ToastDuration.Short, isSuccess ? 20 : 14);

			await MainThread.InvokeOnMainThreadAsync(async () =>
			{
				await toast.Show();
			});
		}
	}
}
