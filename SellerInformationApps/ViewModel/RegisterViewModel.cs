using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using SellerInformationApps.ServiceHelper;
using ServiceHelper.Authentication;
using System.Text;
using System.Windows.Input;

namespace SellerInformationApps.Pages
{
	public partial class RegisterViewModel : Authentication
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
		private string password;

		[ObservableProperty]
		private string verifyPassword;

		[ObservableProperty]
		[JsonConverter(typeof(CustemDateTimeConverter))]
		private DateTime? age = DateTime.Now;

		private DateTime _currentDate = DateTime.Now;
		public DateTime CurrentDate
		{
			get => _currentDate;
			set => SetProperty(ref _currentDate, value);
		}

		[RelayCommand]
		public async Task RegisterAsync()
		{
			if (!IsFormValid())
			{
				await App.Current.MainPage.DisplayAlert("Hata", "Lütfen tüm alanları doldurduğunuzdan emin olun.", "Tamam");
				return;
			}

			if (!ArePasswordsMatching())
			{
				await App.Current.MainPage.DisplayAlert("Hata", "Şifreler eşleşmiyor.", "Tamam");
				return;
			}

			try
			{
				var user = CreateUser();

				if (user == null)
				{
					await App.Current.MainPage.DisplayAlert("Hata", "Kullanıcı bilgileri oluşturulurken bir hata oluştu.", "Tamam");
					return;
				}

				var httpClient = HttpClientFactory.Create("https://e669-37-130-115-34.ngrok-free.app");
				string url = "https://e669-37-130-115-34.ngrok-free.app/RegisterAPI/RegisterUser";
				var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

				using (var response = await httpClient.PostAsync(url, content))
				{
					await HandleResponseAsync(response);
				}
			}
			catch (Exception ex)
			{
				await App.Current.MainPage.DisplayAlert("Hata", $"Hata Oluştu: {ex.Message}", "Tamam");
			}
		}

		private bool IsFormValid()
		{
			return !(string.IsNullOrWhiteSpace(FirstName) ||
				   string.IsNullOrWhiteSpace(LastName) ||
				   string.IsNullOrWhiteSpace(UserName) ||
				   string.IsNullOrWhiteSpace(Email) ||
				   string.IsNullOrWhiteSpace(Password) ||
				   !Age.HasValue);
		}

		private bool ArePasswordsMatching()
		{
			return Password == VerifyPassword;
		}

		private User CreateUser()
		{
			return new User
			{
				FirstName = FirstName,
				LastName = LastName,
				UserName = UserName,
				Email = Email,
				Password = Password,
				Age = Age.Value
			};
		}

		private async Task HandleResponseAsync(HttpResponseMessage response)
		{
			if (response.IsSuccessStatusCode)
			{
				string json = await response.Content.ReadAsStringAsync();
				var apiResponse = JsonConvert.DeserializeObject<RegisterApiResponse>(json);

				if (apiResponse.Success)
				{
					await App.Current.MainPage.DisplayAlert("Başarı", "Kayıt başarılı!", "Tamam");
					await Shell.Current.GoToAsync("//LoginPage");
				}
				else
				{
					await App.Current.MainPage.DisplayAlert("Hata", apiResponse.ErrorMessage, "Tamam");
				}
			}
			else
			{
				await App.Current.MainPage.DisplayAlert("Hata", $"HTTP İsteği Başarısız: {response.StatusCode}", "Tamam");
			}
		}

		public IRelayCommand LoginCommand { get; }

		public RegisterViewModel()
		{
			CurrentDate = DateTime.Now;
			LoginCommand = new AsyncRelayCommand(NavigateToLoginPageAsync);
		}

		public event PropertyChangingEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangingEventArgs(propertyName));
		}

		[RelayCommand]
		private async Task NavigateToLoginPageAsync()
		{
			await Shell.Current.GoToAsync("//LoginPage");
		}
	}
}
