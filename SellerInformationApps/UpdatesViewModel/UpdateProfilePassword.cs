using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
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
				await ShowToast("Eski Şifre ile Yeni Şifre Aynı Olamaz");
				return;
			}

			if (!IsFormValid())
			{
				await ShowToast("Tüm alanları doldurunuz ve şifreler eşleşmelidir");
				return;
			}

			if (!IsCorrectOldPassword())
			{
				await ShowToast("Mevcut şifreniz yanlış");
				return;
			}

			try
			{
				var user = PrepareUserData();
				if (user == null)
				{
					await ShowToast("Kullanıcı bilgileri güncellenirken bir hata oluştu");
					return;
				}

				string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{UserName}:{UsedPassword}"));

				var httpClient = HttpClientFactory.Create("https://c846-37-130-115-91.ngrok-free.app");
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

				string url = "https://c846-37-130-115-91.ngrok-free.app/UserUpdateApi/UpdatePassword";
				var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

				using (var response = await httpClient.PostAsync(url, content))
				{
					await HandleResponseAsync(response);
				}
			}
			catch (Exception ex)
			{
				await ShowToast($"Hata oluştu: {ex.Message}");
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
					await ShowToast("Şifre başarıyla güncellendi");
					Preferences.Set("Password", NewPassword);
					IsPasswordUpdated = true;
					await Shell.Current.GoToAsync("//ProfilePage");
				}
				else
				{
					await ShowToast("Şifre güncelleme başarısız oldu");
					IsPasswordUpdated = false;
				}
			}
			else
			{
				await ShowToast($"Http isteği başarısız oldu: {response.StatusCode}");
			}
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
