using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using SellerInformationApps.PopUps.ForgetPasswordPopUps;
using ServiceHelper.Authentication;
using System.Text;

namespace SellerInformationApps.UpdatesViewModel.ForgetPasswordViewModels
{
	public partial class VerificationCodeEntryViewModel : Authentication
	{
		private readonly Popup _popup;

		private string UserName = Preferences.Get("UserName", string.Empty);


		[ObservableProperty]
		private string validationCode;

		public IRelayCommand SubmitCommand { get; }
		public IRelayCommand CancelCommand { get; }

		public VerificationCodeEntryViewModel(Popup popup)
		{
			_popup = popup;
			SubmitCommand = new AsyncRelayCommand(SubmitAsync);
			CancelCommand = new RelayCommand(ClosePopup);
		}

		private void ClosePopup()
		{
			_popup?.Close();
		}

		public async Task SubmitAsync()
		{
			if (ValidationCode == null)
			{
				await ShowToast("Lütfen Doğrulama Kodunu Giriniz");
				return;
			}

			try
			{
				var validationCode = CreateVerificationCode();

				if (validationCode == null)
				{
					await ShowToast("Geçersiz doğrulama kodu");
					return;
				}

				var httpClient = HttpClientFactory.Create("https://c846-37-130-115-91.ngrok-free.app");
				string url = "https://c846-37-130-115-91.ngrok-free.app/RegisterAndLoginApi/ValidateCode";
				var content = new StringContent(JsonConvert.SerializeObject(validationCode), Encoding.UTF8, "application/json");

				using (var response = await httpClient.PostAsync(url, content))
				{
					if (response.IsSuccessStatusCode)
					{
						string json = await response.Content.ReadAsStringAsync();
						var apiResponse = JsonConvert.DeserializeObject<VerificationCodeApiResponse>(json);

						if (apiResponse.Success)
						{
							await Shell.Current.DisplayAlert("Başarılı", "Doğrulama Kodu Doğrulandı", "Tamam");

							await Task.Run(() => ClosePopup());

							var popup = new ChangePasswordPopUp();

							Application.Current.MainPage.ShowPopup(popup);
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
			catch (HttpRequestException httpEx)
			{
				await ShowToast("İnternet bağlantınızı kontrol edin: " + httpEx.Message);
			}
			catch (Exception ex)
			{
				await ShowToast($"Bir Hata Oluştu: {ex.Message}");
			}
		}

		private VerificationCodeModel CreateVerificationCode()
		{
			return new VerificationCodeModel
			{
				UserName = UserName,
				ValidationCode = ValidationCode
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
