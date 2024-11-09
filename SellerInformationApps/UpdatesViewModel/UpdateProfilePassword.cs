using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using ServiceHelper.Alerts;
using ServiceHelper.Authentication;
using System.Net.Http.Headers;
using System.Text;

namespace SellerInformationApps.UpdatesViewModel
{
	public partial class UpdateProfilePassword : Authentication
	{
		public AlertsHelper alertsHelper = new AlertsHelper();

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
			// Validation Checks
			if (string.IsNullOrWhiteSpace(UserName))
			{
				await alertsHelper.ShowSnackBar("Kullanıcı adı boş olamaz", true);
				return;
			}

			if (string.IsNullOrWhiteSpace(OldPassword))
			{
				await alertsHelper.ShowSnackBar("Eski şifre boş olamaz", true);
				return;
			}

			if (string.IsNullOrWhiteSpace(NewPassword))
			{
				await alertsHelper.ShowSnackBar("Yeni şifre boş olamaz", true);
				return;
			}

			if (NewPassword != VerifyNewPassword)
			{
				await alertsHelper.ShowSnackBar("Yeni şifre ve doğrulama şifresi eşleşmiyor", true);
				return;
			}

			if (IsOldPasswordUsed())
			{
				await alertsHelper.ShowSnackBar("Eski şifre ile yeni şifre aynı olamaz", true);
				return;
			}

			if (!IsCorrectOldPassword())
			{
				await alertsHelper.ShowSnackBar("Mevcut şifreniz yanlış", true);
				return;
			}

			try
			{
				var user = PrepareUserData();
				if (user == null)
				{
					await alertsHelper.ShowSnackBar("Kullanıcı bilgileri hazırlanırken bir hata oluştu", true);
					return;
				}

				string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{UserName}:{UsedPassword}"));
				var httpClient = HttpClientFactory.Create("https://1304-37-130-115-91.ngrok-free.app");
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

				string url = "https://1304-37-130-115-91.ngrok-free.app/UserUpdateApi/UpdatePassword";
				var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

				using (var request = new HttpRequestMessage(HttpMethod.Post, url))
				{
					request.Content = content;

					using (var response = await httpClient.SendAsync(request))
					{
						string json = await response.Content.ReadAsStringAsync();
						var apiResponse = JsonConvert.DeserializeObject<UpdatePasswordApiResponse>(json);

						if (response.IsSuccessStatusCode && apiResponse?.Success == true)
						{
							await alertsHelper.ShowSnackBar("Şifre başarıyla güncellendi");
							Preferences.Set("Password", NewPassword);
							IsPasswordUpdated = true;
							await Shell.Current.GoToAsync("//ProfilePage");
						}
						else
						{
							await alertsHelper.ShowSnackBar(apiResponse?.ErrorMessage ?? "Şifre güncelleme başarısız oldu", true);
							IsPasswordUpdated = false;
						}
					}
				}
			}
			catch (HttpRequestException httpEx)
			{
				await alertsHelper.ShowSnackBar($"Ağ bağlantısında hata oluştu: {httpEx.Message}", true);
			}
			catch (JsonException jsonEx)
			{
				await alertsHelper.ShowSnackBar($"Veri işleme hatası: {jsonEx.Message}", true);
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"Bir hata oluştu: {ex.Message}", true);
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
	}
}
