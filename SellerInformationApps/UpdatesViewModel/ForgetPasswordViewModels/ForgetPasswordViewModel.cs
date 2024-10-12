using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SellerInformationApps.Models;
using System.Text;
using Newtonsoft.Json;
using CommunityToolkit.Maui.Views;
using SellerInformationApps.PopUps.ForgetPasswordPopUps;
using PraPazar.ServiceHelper;

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
				await Shell.Current.DisplayAlert("Hata", "Lütfen tüm alanları doldurduğunuzdan emin olun", "Tamam");
				return;
			}

			try
			{
				var forgetPassword = CreateForgetPassword();

				if (forgetPassword == null)
				{
					await Shell.Current.DisplayAlert("Hata", "Bir hata oluştu", "Tamam");
					return;
				}

				var httpClient = HttpClientFactory.Create("https://c177-37-130-115-91.ngrok-free.app");
				string url = "https://c177-37-130-115-91.ngrok-free.app/RegisterAndLoginApi/ForgetPassword";

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
							await Shell.Current.DisplayAlert("Hata", apiResponse.ErrorMessage, "Tamam");
						}
					}
					else
					{
						await Shell.Current.DisplayAlert("Hata", $"HTTP isteği başarısız oldu: {response.StatusCode}", "Tamam");
					}
				}
			}
			catch (Exception ex)
			{
				await Shell.Current.DisplayAlert("Hata", $"Bir hata oluştu: {ex.Message}", "Tamam");
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
