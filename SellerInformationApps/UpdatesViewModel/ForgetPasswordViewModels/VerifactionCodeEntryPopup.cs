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
				await Shell.Current.DisplayAlert("Hata", "Lütfen Doğrulama Kodunu Giriniz", "Tamam");
				return;
			}

			try
			{
				var validationCode = CreateVerificationCode();

				if (validationCode == null)
				{
					await Shell.Current.DisplayAlert("Hata", "Geçersiz doğrulama kodu", "Tamam");
					return;
				}

				var httpClient = HttpClientFactory.Create("https://c177-37-130-115-91.ngrok-free.app");
				string url = "https://c177-37-130-115-91.ngrok-free.app/RegisterAndLoginApi/ValidateCode";
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
							await Shell.Current.DisplayAlert("Hata", apiResponse.ErrorMessage, "Tamam");
						}
					}
					else
					{
						await Shell.Current.DisplayAlert("Hata", $"HTTP isteği başarısız oldu: {response.StatusCode}", "Tamam");
					}
				}
			}
			catch (HttpRequestException httpEx)
			{
				await Shell.Current.DisplayAlert("Hata", "İnternet bağlantınızı kontrol edin: " + httpEx.Message, "Tamam");
			}
			catch (Exception ex)
			{
				await Shell.Current.DisplayAlert("Hata", $"Bir Hata Oluştu: {ex.Message}", "Tamam");
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
