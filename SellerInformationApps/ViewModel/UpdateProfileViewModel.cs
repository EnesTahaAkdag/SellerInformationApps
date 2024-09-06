using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using ServiceHelper.Authentication;
using System.Net.Http.Headers;
using System.Text;

namespace SellerInformationApps.ViewModel
{
	//[QueryProperty(nameof(FirstName), "FirstName")]
	//[QueryProperty(nameof(LastName), "LastName")]
	//[QueryProperty(nameof(UserName), "UserName")]
	//[QueryProperty(nameof(Email), "Email")]
	//[QueryProperty(nameof(Age), "Age")]
		[QueryProperty(nameof(FirstName), nameof(FirstName))]
		[QueryProperty(nameof(LastName), nameof(LastName))]
		[QueryProperty(nameof(UserName), nameof(UserName))]
		[QueryProperty(nameof(Email), nameof(Email))]
		[QueryProperty(nameof(Age), nameof(Age))]

	public partial class UpdateProfileViewModel : Authentication
	{
		[ObservableProperty]
		private string firstName;

		[ObservableProperty]
		private string lastName;

		[ObservableProperty]
		private string userName;

		[ObservableProperty]
		private string email;

		[ObservableProperty]
		private DateTime age;

		public UserProfileData UserProfile { get; internal set; }

		public async Task WriteData(UserProfileData userProfileData)
		{
			if (userProfileData == null)
			{
				await Shell.Current.DisplayAlert("Hata", "Veriler Gelmedi", "Tamam");
			}
			else
			{
				FirstName = userProfileData.FirstName;
				LastName = userProfileData.LastName;
				UserName = userProfileData.UserName;
				Email = userProfileData.Email;
				Age = userProfileData.Age.Value;
			}
		}

		[RelayCommand]
		public async Task SendUpdatedData()
		{
			if (!IsFormValid())
			{
				await Shell.Current.DisplayAlert("Hata", "Lütfen Tüm Alanları Doldurunuz", "Tamam");
				return;
			}

			try
			{
				var user = ReadData();

				if (user == null)
				{
					await Shell.Current.DisplayAlert("Hata", "Kullanıcı Bilgileri Güncellenirken Bir Hata Oluştu", "Tamam");
					return;
				}

				var httpClient = HttpClientFactory.Create("https://f038-37-130-115-34.ngrok-free.app");
				string url = "https://f038-37-130-115-34.ngrok-free.app/UserProfileData/UpdateProfile";
				var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

				using (var response = await httpClient.PostAsync(url, content))
				{
					await HandleResponseAsync(response);
				}
		}
			catch (Exception ex)
			{
				await Shell.Current.DisplayAlert("Hata", $"Hata Oluştu: {ex.Message}", "Tamam");
			}
		}

		public bool IsFormValid()
		{
			return !(string.IsNullOrWhiteSpace(FirstName) ||
					 string.IsNullOrWhiteSpace(LastName) ||
					 string.IsNullOrWhiteSpace(UserName) ||
					 string.IsNullOrWhiteSpace(Email) ||
					 Age == default(DateTime));
		}

		public UserProfileData ReadData()
		{
			return new UserProfileData
			{
				FirstName = FirstName,
				LastName = LastName,
				UserName = UserName,
				Email = Email,
				Age = Age
			};
		}

		private async Task HandleResponseAsync(HttpResponseMessage response)
		{
			if (response.IsSuccessStatusCode)
			{
				string json = await response.Content.ReadAsStringAsync();
				var apiResponse = JsonConvert.DeserializeObject<UserApiResponse>(json);
				if (apiResponse.Success)
				{
					await Shell.Current.DisplayAlert("Başarılı", "Güncelleme Başarılı", "Tamam");
					await Shell.Current.GoToAsync("//ProfilePage");
				}
				else
				{
					await Shell.Current.DisplayAlert("Hata", "Güncelleme başarısız oldu.", "Tamam");
				}
			}
			else
			{
				await Shell.Current.DisplayAlert("Hata", $"Http İsteği Başarısız: {response.StatusCode}", "Tamam");
			}
		}
	}
}
