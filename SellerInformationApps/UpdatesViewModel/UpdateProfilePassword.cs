using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using ServiceHelper.Authentication;
using System.Net.Http.Headers;
using System.Text;

namespace SellerInformationApps.UpdatesViewModel
{
	public partial class UpdateProfilePassword : Authentication
	{
		[ObservableProperty]
		private string userName = Preferences.Get("UserName", string.Empty);

		[ObservableProperty]
		private string usedPassword = Preferences.Get("Password", string.Empty);

		[ObservableProperty]
		private string oldPassword;

		[ObservableProperty]
		private string newPassword;

		[ObservableProperty]
		private string verifyNewPassword;

		[RelayCommand]
		public async Task SubmitPasswordAsync()
		{
			if (!IsFormValid())
			{
				await Shell.Current.DisplayAlert("Hata", "Tüm alanları doldurunuz ve şifreler eşleşmelidir", "Tamam");
				return;
			}

			if (!IsVerifyPassword())
			{
				await Shell.Current.DisplayAlert("Hata", "Mevcut şifreniz yanlış", "Tamam");
				return;
			}

			try
			{
				var user = ReadData();
				if (user == null)
				{
					await Shell.Current.DisplayAlert("Hata", "Kullanıcı bilgileri güncellenirken bir hata oluştu", "Tamam");
					return;
				}

				var userNames = Preferences.Get("UserName", string.Empty);
				var password = Preferences.Get("Password", string.Empty);

				string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userNames}:{password}"));

				var httpClient = HttpClientFactory.Create("https://8b27-37-130-115-34.ngrok-free.app");
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",authHeaderValue);
				string url = "https://8b27-37-130-115-34.ngrok-free.app/UserPasswordApi/UpdatePassword";
				var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

				using (var response = await httpClient.PostAsync(url, content))
				{
					await HandleResponseAsync(response);
				}
			}
			catch (Exception ex)
			{
				await Shell.Current.DisplayAlert("Hata", $"Hata oluştu: {ex.Message}", "Tamam");
			}
		}

		private bool IsFormValid()
		{
			return !(
				string.IsNullOrWhiteSpace(UserName) ||
				string.IsNullOrWhiteSpace(OldPassword) ||
				string.IsNullOrWhiteSpace(NewPassword) ||
				string.IsNullOrWhiteSpace(VerifyNewPassword) ||
				NewPassword != VerifyNewPassword
			);
		}

		private bool IsVerifyPassword()
		{
			return UsedPassword == OldPassword;
		}

		private UpdateUserPassword ReadData()
		{
			return new UpdateUserPassword
			{
				UserName = UserName,
				Password = NewPassword,
			};
		}

		private async Task HandleResponseAsync(HttpResponseMessage response)
		{
			if (response.IsSuccessStatusCode)
			{
				string json = await response.Content.ReadAsStringAsync();
				var apiResponse = JsonConvert.DeserializeObject<UpdatePasswordApiResponse>(json);
				if (apiResponse.Success)
				{
					await Shell.Current.DisplayAlert("Başarı", "Şifre başarıyla güncellendi", "Tamam");
					Preferences.Set("Password", NewPassword);
					await Shell.Current.GoToAsync("//ProfilePage");
				}
				else
				{
					await Shell.Current.DisplayAlert("Hata", "Şifre güncelleme başarısız oldu", "Tamam");
				}
			}
			else
			{
				await Shell.Current.DisplayAlert("Hata", $"Http isteği başarısız oldu: {response.StatusCode}", "Tamam");
			}
		}
	}
}
