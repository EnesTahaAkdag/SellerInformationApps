using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SellerInformationApps.Models;
using ServiceHelper.Authentication;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using PraPazar.ServiceHelper;

namespace SellerInformationApps.UpdatesViewModel
{
	public partial class ForgetPasswordViewModel : ObservableObject
	{

		[ObservableProperty]
		private string userName;

		public IRelayCommand SubmitCommand { get; }
		public IRelayCommand CancelCommand { get; }

		public ForgetPasswordViewModel()
		{
			SubmitCommand = new AsyncRelayCommand(SubmitCommandAsync);
			CancelCommand = new RelayCommand(Cancel);
		}

		private void Cancel()
		{
			Shell.Current.Navigation.PopAsync();  // Popup'u kapatma işlemi
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

				var httpClient = HttpClientFactory.Create("https://0ad8-37-130-115-91.ngrok-free.app");
				string url = "https://0ad8-37-130-115-91.ngrok-free.app/RegisterAndLoginApi/ForgetPassword";

				var content = new StringContent(JsonConvert.SerializeObject(forgetPassword), Encoding.UTF8, "application/json");

				using (var response = await httpClient.PostAsync(url, content))
				{
					await HandleResponseAsync(response);
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

		private async Task HandleResponseAsync(HttpResponseMessage response)
		{
			try
			{
				if (response.IsSuccessStatusCode)
				{
					string json = await response.Content.ReadAsStringAsync();
					var apiResponse = JsonConvert.DeserializeObject<ForgetPasswordApiResponse>(json);

					if (apiResponse.Success)
					{
						await Shell.Current.DisplayAlert("Başarılı", "Doğrulama kodu e-posta adresinize gönderildi", "Tamam");
					}
					else
					{
						await Shell.Current.DisplayAlert("Hata", apiResponse.ErrorMessage, "Tamam");
					}
				}
				else
				{
					string errorResponse = await response.Content.ReadAsStringAsync();
					await Shell.Current.DisplayAlert("Hata", $"HTTP isteği başarısız oldu: {response.StatusCode}\n{errorResponse}", "Tamam");
				}
			}
			catch (Exception ex)
			{
				await Shell.Current.DisplayAlert("Hata", $"Yanıt işlenirken bir hata oluştu: {ex.Message}", "Tamam");
			}
		}
	}
}
