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

		public AlertsHelper alertsHelper = new AlertsHelper();
		private string UserName = Preferences.Get("UserName", string.Empty);

		[ObservableProperty]
		private string validationCode;

		public int RemainingTime
		{
			get => _remainingTime;
			set => SetProperty(ref _remainingTime, value);
		}

		public IRelayCommand SubmitCommand { get; }
		public IRelayCommand CancelCommand { get; }

		public VerificationCodeEntryViewModel(Popup popup)
		{
			_popup = popup;
			SubmitCommand = new AsyncRelayCommand(SubmitAsync);
			CancelCommand = new RelayCommand(ClosePopup);

			RemainingTime = 120; 
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
					await alertsHelper.ShowSnackBar("Doğrulama kodunun süresi doldu, lütfen tekrar deneyin.", true);
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
				await alertsHelper.ShowSnackBar("Lütfen Doğrulama Kodunu Giriniz", true);
				return;
			}

			try
			{
				var validationCode = CreateVerificationCode();

				if (validationCode == null)
				{
					await alertsHelper.ShowSnackBar("Geçersiz doğrulama kodu", true);
					return;
				}

				var httpClient = HttpClientFactory.Create("https://5462-37-130-115-91.ngrok-free.app");
				string url = "https://5462-37-130-115-91.ngrok-free.app/RegisterAndLoginApi/ValidateCode";
				var content = new StringContent(JsonConvert.SerializeObject(validationCode), Encoding.UTF8, "application/json");

				using (var response = await httpClient.PostAsync(url, content))
				{
					if (response.IsSuccessStatusCode)
					{
						string json = await response.Content.ReadAsStringAsync();
						var apiResponse = JsonConvert.DeserializeObject<VerificationCodeApiResponse>(json);

						if (apiResponse.Success)
						{
							await alertsHelper.ShowSnackBar("Doğrulama Kodu Doğrulandı");
							ClosePopup();

							var popup = new ChangePasswordPopUp();
							Application.Current.MainPage.ShowPopup(popup);
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
			catch (HttpRequestException httpEx)
			{
				await alertsHelper.ShowSnackBar("İnternet bağlantınızı kontrol edin: " + httpEx.Message, true);
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"Bir Hata Oluştu: {ex.Message}", true);
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
	}
}
