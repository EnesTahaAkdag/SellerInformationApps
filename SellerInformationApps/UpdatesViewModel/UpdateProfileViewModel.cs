using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using SellerInformationApps.PopUps;
using ServiceHelper.Alerts;
using System.Net.Http.Headers;
using System.Text;

namespace SellerInformationApps.UpdatesViewModel
{
	public partial class UpdateProfileViewModel : ObservableObject
	{
		[ObservableProperty] private string firstName;
		[ObservableProperty] private string lastName;
		[ObservableProperty] private string userName;
		[ObservableProperty] private string email;
		[ObservableProperty] private DateTime? age;
		[ObservableProperty] private string profileImageBase64;

		private readonly AlertsHelper alertsHelper = new AlertsHelper();
		public UserProfileData UserProfile { get; set; }


		public Popup _popUp;

		public IRelayCommand UpdateProfileCommand { get; }
		public IRelayCommand CancelUpdatePopUpCommand { get; }

		public UpdateProfileViewModel(Popup popup)
		{
			_popUp = popup;
			UpdateProfileCommand = new AsyncRelayCommand(SubmitAsync);
			CancelUpdatePopUpCommand = new RelayCommand(CancelPopup);
		}

		private void CancelPopup()
		{
			_popUp?.Close();
		}

		public async Task WriteData(UserProfileData updateUserData)
		{
			if (updateUserData == null)
			{
				await alertsHelper.ShowSnackBar("Güncellenen veriler Profil Sayfasına Gelmedi", true);
				return;
			}

			try
			{
				FirstName = updateUserData.FirstName ?? string.Empty;
				LastName = updateUserData.LastName ?? string.Empty;
				UserName = updateUserData.UserName ?? string.Empty;
				Email = updateUserData.Email ?? string.Empty;
				Age = updateUserData.Age ?? DateTime.Now;
				ProfileImageBase64 = !string.IsNullOrWhiteSpace(updateUserData.ProfileImageBase64)
					? updateUserData.ProfileImageBase64
					: "profilephotots.png";
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"Veriler geldi, ancak ekrana yazdırılırken hata oluştu: {ex.Message}", true);
			}
		}

		[RelayCommand]
		public async Task SubmitAsync()
		{
			if (!IsFormValid())
			{
				await alertsHelper.ShowSnackBar("Lütfen tüm alanları doldurunuz", true);
				return;
			}

			try
			{
				var user = ReadData();

				var userName = Preferences.Get("UserName", string.Empty);
				var password = Preferences.Get("Password", string.Empty);

				if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
				{
					await alertsHelper.ShowSnackBar("Kullanıcı adı veya şifre eksik", true);
					return;
				}

				var client = HttpClientFactory.Create("https://de29-37-130-115-91.ngrok-free.app");
				string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

				string url = "https://de29-37-130-115-91.ngrok-free.app/UserUpdateApi/EditUserData";
				var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

				using (var response = await client.PostAsync(url, content))
				{
					if (response.IsSuccessStatusCode)
					{
						await alertsHelper.ShowSnackBar("Profil başarıyla güncellendi", false);
					}
					else
					{
						await alertsHelper.ShowSnackBar("Profil güncellenemedi", true);
					}
				}
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"Bir hata oluştu: {ex.Message}", true);
			}
		}

		private bool IsFormValid()
		{
			return !string.IsNullOrWhiteSpace(FirstName) &&
				   !string.IsNullOrWhiteSpace(LastName) &&
				   !string.IsNullOrWhiteSpace(Email) &&
				   Age != null;
		}

		private UserProfileData ReadData()
		{
			return new UserProfileData
			{
				FirstName = FirstName,
				LastName = LastName,
				UserName = UserName,
				Email = Email,
				Age = Age,
			};
		}
	}
}
