using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using SellerInformationApps.PopUps.ForgetPasswordPopUps;
using ServiceHelper.Alerts;
using System.Text;
using System.Timers;

namespace SellerInformationApps.UpdatesViewModel.ForgetPasswordViewModels
{
	public partial class VerificationCodeEntryViewModel : ObservableObject
	{
		private readonly Popup _popup;
		private System.Timers.Timer _timer;
		private int _remainingTime;

		private readonly AlertsHelper _alertsHelper = new AlertsHelper();
		private readonly string _userName = Preferences.Get("UserName", string.Empty);

		[ObservableProperty]
		private string _validationCode;

		public int RemainingTime
		{
			get => _remainingTime;
			set => SetProperty(ref _remainingTime, value);
		}

		public IRelayCommand SubmitCommand { get; }
		public IRelayCommand CancelCommand { get; }

		public VerificationCodeEntryViewModel(Popup popup)
		{
			_popup = popup ?? throw new ArgumentNullException(nameof(popup));
			SubmitCommand = new AsyncRelayCommand(SubmitAsync);
			CancelCommand = new RelayCommand(ClosePopup);

			RemainingTime = 120;  // Doğrulama kodu süresi
			StartTimer();
		}

		private void StartTimer()
		{
			_timer = new System.Timers.Timer(1000);
			_timer.Elapsed += OnTimedEvent;
			_timer.AutoReset = true;
			_timer.Enabled = true;
		}

		private void OnTimedEvent(object source, ElapsedEventArgs e)
		{
			if (RemainingTime > 0)
			{
				RemainingTime--;
			}
			else
			{
				_timer.Stop();
				_popup.Dispatcher.Dispatch(async () =>
				{
					await _alertsHelper.ShowSnackBar("Doğrulama kodunun süresi doldu, lütfen tekrar deneyin.", true);
					ClosePopup();
				});
			}
		}

		private void ClosePopup()
		{
			_timer?.Stop();
			_popup?.Close();
		}

		public async Task SubmitAsync()
		{
			if (string.IsNullOrWhiteSpace(ValidationCode))
			{
				await _alertsHelper.ShowSnackBar("Lütfen doğrulama kodunu giriniz.", true);
				return;
			}

			try
			{
				var validationCodeModel = CreateVerificationCode();

				if (validationCodeModel == null)
				{
					await _alertsHelper.ShowSnackBar("Geçersiz doğrulama kodu, lütfen tekrar deneyiniz.", true);
					return;
				}

				var httpClient = HttpClientFactory.Create("https://1304-37-130-115-91.ngrok-free.app");
				string url = "https://1304-37-130-115-91.ngrok-free.app/RegisterAndLoginApi/ValidateCode";
				var content = new StringContent(JsonConvert.SerializeObject(validationCodeModel), Encoding.UTF8, "application/json");

				using (var response = await httpClient.PostAsync(url, content))
				{
					if (response.IsSuccessStatusCode)
					{
						string json = await response.Content.ReadAsStringAsync();
						var apiResponse = JsonConvert.DeserializeObject<VerificationCodeApiResponse>(json);

						if (apiResponse?.Success == true)
						{
							await _alertsHelper.ShowSnackBar("Doğrulama kodu doğrulandı.");
							ClosePopup();

							var popup = new ChangePasswordPopUp();
							Application.Current.MainPage.ShowPopup(popup);
						}
						else
						{
							var errorMessage = apiResponse?.ErrorMessage ?? "Bilinmeyen bir hata oluştu.";
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

		private VerificationCodeModel CreateVerificationCode()
		{
			return new VerificationCodeModel
			{
				UserName = _userName,
				ValidationCode = ValidationCode
			};
		}
	}
}
