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
		public bool IsPasswordUpdated;

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
			if (IsOldPasswordUsed())
			{
				await Shell.Current.DisplayAlert("Hata", "Eski Şifre ile Yeni Şifre Aynı Olamaz", "Tamam");
				return;
			}

			if (!IsFormValid())
			{
				await Shell.Current.DisplayAlert("Hata", "Tüm alanları doldurunuz ve şifreler eşleşmelidir", "Tamam");
				return;
			}

			if (!IsCorrectOldPassword())
			{
				await Shell.Current.DisplayAlert("Hata", "Mevcut şifreniz yanlış", "Tamam");
				return;
			}

			try
			{
				var user = PrepareUserData();
				if (user == null)
				{
					await Shell.Current.DisplayAlert("Hata", "Kullanıcı bilgileri güncellenirken bir hata oluştu", "Tamam");
					return;
				}

				string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{UserName}:{UsedPassword}"));

				var httpClient = HttpClientFactory.Create("https://778d-37-130-115-34.ngrok-free.app");
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

				string url = "https://778d-37-130-115-34.ngrok-free.app/UserUpdateApi/UpdatePassword";
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

		private bool IsOldPasswordUsed()
		{
			return UsedPassword == NewPassword;
		}

		private bool IsCorrectOldPassword()
		{
			return UsedPassword == OldPassword;
		}

		private UpdateUserPassword PrepareUserData()
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
					IsPasswordUpdated = true;
					await Shell.Current.GoToAsync("//ProfilePage");
				}
				else
				{
					await Shell.Current.DisplayAlert("Hata", "Şifre güncelleme başarısız oldu", "Tamam");
					IsPasswordUpdated = false;
				}
			}
			else
			{
				await Shell.Current.DisplayAlert("Hata", $"Http isteği başarısız oldu: {response.StatusCode}", "Tamam");
			}
		}
	}
}
