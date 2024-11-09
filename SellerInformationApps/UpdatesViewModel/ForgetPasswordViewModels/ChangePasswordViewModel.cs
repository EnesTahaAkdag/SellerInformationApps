using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using ServiceHelper.Authentication;
using System.Text;
using ServiceHelper.Alerts;

namespace SellerInformationApps.UpdatesViewModel.ForgetPasswordViewModels
{
	public partial class ChangePasswordViewModel : Authentication
	{
		private readonly AlertsHelper _alertsHelper = new AlertsHelper();
		private readonly Popup _popup;

		[ObservableProperty]
		private string password;

		[ObservableProperty]
		private string verifyPassword;

		private readonly string _userName = Preferences.Get("UserName", string.Empty);

		public IRelayCommand UpdatePasswordCommand { get; }
		public IRelayCommand CancelCommand { get; }

		public ChangePasswordViewModel(Popup popup)
		{
			_popup = popup ?? throw new ArgumentNullException(nameof(popup));
			UpdatePasswordCommand = new AsyncRelayCommand(SubmitCommandAsync);
			CancelCommand = new RelayCommand(ClosePopup);
		}

		private void ClosePopup()
		{
			_popup?.Close();
		}

		[RelayCommand]
		private async Task SubmitCommandAsync()
		{
			if (!await IsFormValid())
			{
				await _alertsHelper.ShowSnackBar("Lütfen tüm alanları doldurduğunuzdan emin olun.", true);
				return;
			}

			if (string.IsNullOrEmpty(_userName))
			{
				await _alertsHelper.ShowSnackBar("Kullanıcı adı boş olamaz. Lütfen tekrar giriş yapın.", true);
				return;
			}

			try
			{
				var changePasswordModel = CreateNewPasswordModel();

				if (changePasswordModel == null)
				{
					await _alertsHelper.ShowSnackBar("Şifre değişiklik modeli oluşturulamadı.", true);
					return;
				}

				string url = "https://1304-37-130-115-91.ngrok-free.app/RegisterAndLoginApi/ChangePassword";
				var content = new StringContent(JsonConvert.SerializeObject(changePasswordModel), Encoding.UTF8, "application/json");
				var httpClient = HttpClientFactory.Create("https://1304-37-130-115-91.ngrok-free.app");

				using (var response = await httpClient.PostAsync(url, content))
				{
					string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
					var apiResponse = JsonConvert.DeserializeObject<ChancePasswordApiResponse>(json);

					if (response.IsSuccessStatusCode && apiResponse?.Success == true)
					{
						await _alertsHelper.ShowSnackBar("Şifreniz başarıyla değiştirildi. Lütfen giriş yapın.");
						Preferences.Remove("UserName");
						ClosePopup();
					}
					else
					{
						var errorMessage = apiResponse?.ErrorMessage ?? "Bilinmeyen bir hata oluştu.";
						await _alertsHelper.ShowSnackBar(errorMessage, true);
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

		private async Task<bool> IsFormValid()
		{
			if (string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(VerifyPassword))
				return false;

			if (Password != VerifyPassword)
			{
				await _alertsHelper.ShowSnackBar("Şifreler eşleşmiyor.", true);
				return false;
			}

			if (Password.Length < 6)
			{
				await _alertsHelper.ShowSnackBar("Şifre en az 6 karakter olmalıdır.", true);
				return false;
			}

			return true;
		}


		private ChancePasswordModel CreateNewPasswordModel()
		{
			return new ChancePasswordModel
			{
				UserName = _userName,
				Password = Password
			};
		}
	}
}
