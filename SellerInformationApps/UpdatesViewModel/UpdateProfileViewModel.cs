using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
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
		[ObservableProperty] private ImageSource profileImage;

		public AlertsHelper alertsHelper = new AlertsHelper();
		public UserProfileData UserProfile { get; set; }

		public async Task WriteData(UserProfileData updateUserData)
		{
			if (updateUserData == null)
			{
				await alertsHelper.ShowSnackBar("Güncellenen veriler Profil Sayfasına Gelmedi", true);
			}
			else
			{
				try
				{
					FirstName = updateUserData.FirstName ?? string.Empty;
					OnPropertyChanged(nameof(FirstName));

					LastName = updateUserData.LastName ?? string.Empty;
					OnPropertyChanged(nameof(LastName));

					UserName = updateUserData.UserName ?? string.Empty;
					OnPropertyChanged(nameof(UserName));

					Email = updateUserData.Email ?? string.Empty;
					OnPropertyChanged(nameof(Email));

					Age = updateUserData.Age;
					OnPropertyChanged(nameof(Age));

					ProfileImage = updateUserData.ProfileImageBase64 != null
						? ImageSource.FromUri(new Uri(updateUserData.ProfileImageBase64))
						: "default_image.png";
					OnPropertyChanged(nameof(ProfileImage));
				}
				catch (Exception ex)
				{
					await alertsHelper.ShowSnackBar($"Veriler Geldi Ama Ekrana Basılırken hata verdi: {ex.Message}", true);
				}
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

				var client = HttpClientFactory.Create("https://59b7-37-130-115-91.ngrok-free.app");
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
					Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}")));

				var json = JsonConvert.SerializeObject(user);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				using (var response = await client.PutAsync($"/UserUpdateApi/EditUserData", content))
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
