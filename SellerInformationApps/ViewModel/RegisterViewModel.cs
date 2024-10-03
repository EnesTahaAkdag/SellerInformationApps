using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using SellerInformationApps.ServiceHelper;
using ServiceHelper.Authentication;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace SellerInformationApps.ViewModel
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
		private byte[] profileImage;

		[ObservableProperty]
		[JsonConverter(typeof(CustemDateTimeConverter))]
		private DateTime? age = DateTime.Now;

		private DateTime _currentDate = DateTime.Now;
		public DateTime CurrentDate
		{
			get => _currentDate;
			set => SetProperty(ref _currentDate, value);
		}

		public IRelayCommand RegisterUserCommand { get; }

		public RegisterViewModel()
		{
			CurrentDate = DateTime.Now;
			RegisterUserCommand = new AsyncRelayCommand(RegisterAsync);
			LoginCommand = new AsyncRelayCommand(NavigateToLoginPageAsync);
		}

		[RelayCommand]
		public async Task RegisterAsync()
		{
			if (!IsFormValid())
			{
				await Shell.Current.DisplayAlert("Hata", "Lütfen tüm alanları doldurduğunuzdan emin olun.", "Tamam");
				return;
			}

			if (!ArePasswordsMatching())
			{
				await Shell.Current.DisplayAlert("Hata", "Şifreler eşleşmiyor.", "Tamam");
				return;
			}

			try
			{
				var user = CreateUser();

				if (user == null)
				{
					await Shell.Current.DisplayAlert("Hata", "Kullanıcı bilgileri oluşturulurken bir hata oluştu.", "Tamam");
					return;
				}

				var httpClient = HttpClientFactory.Create("https://b8ac-37-130-115-91.ngrok-free.app/");
				string url = "https://b8ac-37-130-115-91.ngrok-free.app/RegisterAndLoginApi/RegisterUser";

				var userJson = JsonConvert.SerializeObject(user);

				using (var request = new HttpRequestMessage(HttpMethod.Post, new Uri(url)))
				{
					var content = new System.Net.Http.StringContent(userJson, Encoding.UTF8, "application/json");
					request.Content = content;
					using (var response = await httpClient.SendAsync(request).ConfigureAwait(false))
					{
						using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
						{
						}
					}
				}
			}
			catch (Exception ex)
			{
				await Shell.Current.DisplayAlert("Hata", $"Hata Oluştu: {ex.Message}", "Tamam");
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
				Age = Age.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm"),
				//ProfileImage = ProfileImage
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
					await Shell.Current.DisplayAlert("Başarı", "Kayıt başarılı!", "Tamam");
					await Shell.Current.GoToAsync("//LoginPage");
				}
				else
				{
					await Shell.Current.DisplayAlert("Hata", apiResponse.ErrorMessage, "Tamam");
				}
			}
			else
			{
				var errorContent = await response.Content.ReadAsStringAsync();
				var errorResponse = JsonConvert.DeserializeObject<RegisterApiResponse>(errorContent);

				if (errorResponse.Errors != null)
				{
					var errorMessages = string.Join("\n", errorResponse.Errors.SelectMany(kvp => kvp.Value));
					await Shell.Current.DisplayAlert("Hata", $"HTTP Hatası: {errorMessages}", "Tamam");
				}
				else
				{
					await Shell.Current.DisplayAlert("Hata", $"HTTP Hatası: {errorContent}", "Tamam");
				}
			}
		}

		public IRelayCommand LoginCommand { get; }

		private async Task NavigateToLoginPageAsync()
		{
			await Shell.Current.GoToAsync("//LoginPage");
		}
	}
}
