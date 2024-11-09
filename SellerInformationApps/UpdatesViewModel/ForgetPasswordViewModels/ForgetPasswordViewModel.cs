using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using SellerInformationApps.PopUps.ForgetPasswordPopUps;
using ServiceHelper.Alerts;
using System.Text;

namespace SellerInformationApps.UpdatesViewModel.ForgetPasswordViewModels
{
	public partial class ForgetPasswordViewModel : ObservableObject
	{
		private readonly AlertsHelper _alertsHelper = new AlertsHelper();
		private readonly Popup _popup;

		[ObservableProperty]
		private string _userName;

		public IRelayCommand SubmitCommand { get; }
		public IRelayCommand CancelCommand { get; }

		public ForgetPasswordViewModel(Popup popup)
		{
			_popup = popup ?? throw new ArgumentNullException(nameof(popup));
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
				await _alertsHelper.ShowSnackBar("Lütfen kullanıcı adını doldurduğunuzdan emin olun.", true);
				return;
			}

			try
			{
				var forgetPasswordModel = CreateForgetPassword();

				if (forgetPasswordModel == null)
				{
					await _alertsHelper.ShowSnackBar("Bir hata oluştu, lütfen tekrar deneyiniz.", true);
					return;
				}

				var httpClient = HttpClientFactory.Create("https://1304-37-130-115-91.ngrok-free.app");
				string url = "https://1304-37-130-115-91.ngrok-free.app/RegisterAndLoginApi/ForgetPassword";
				var content = new StringContent(JsonConvert.SerializeObject(forgetPasswordModel), Encoding.UTF8, "application/json");

				using (var response = await httpClient.PostAsync(url, content))
				{
					if (response.IsSuccessStatusCode)
					{
						string json = await response.Content.ReadAsStringAsync();
						var apiResponse = JsonConvert.DeserializeObject<ForgetPasswordApiResponse>(json);

						if (apiResponse?.Success == true)
						{
							Preferences.Set("UserName", UserName);
							await _alertsHelper.ShowSnackBar("Doğrulama kodu e-posta adresinize gönderildi.");

							var verificationCodeEntryPopup = new VerificationCodeEntryPopup();
							Application.Current.MainPage.ShowPopup(verificationCodeEntryPopup);
							ClosePopup();
						}
						else
						{
							var errorMessage = apiResponse?.ErrorMessage ?? "Bir hata oluştu.";
							await _alertsHelper.ShowSnackBar(errorMessage, true);
						}
					}
					else
					{
						await _alertsHelper.ShowSnackBar($"HTTP isteği başarısız oldu: {response.StatusCode}", true);
					}
				}
			}
			catch (HttpRequestException httpEx)
			{
				await _alertsHelper.ShowSnackBar("İnternet bağlantınızı kontrol edin: " + httpEx.Message, true);
			}
			catch (Exception ex)
			{
				await _alertsHelper.ShowSnackBar($"Bir hata oluştu: {ex.Message}", true);
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
