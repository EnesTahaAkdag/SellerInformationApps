using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using SellerInformationApps.PopUps;
using SellerInformationApps.ServiceHelper;
using ServiceHelper.Authentication;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

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
		private Stream profileImageStream;

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

				var userContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

				MultipartFormDataContent content = new MultipartFormDataContent
				{
					{ userContent, "user" }
				};

				if (ProfileImageStream != null)
				{
					var streamContent = new StreamContent(ProfileImageStream)
					{
						Headers = { ContentType = new MediaTypeHeaderValue("image/jpeg") }
					};
					content.Add(streamContent, "ProfileImage", "profileImage.jpg");
				}

				using (var httpClient = new HttpClient())
				{
					string url = "https://560d-37-130-115-91.ngrok-free.app/RegisterAndLoginApi/RegisterUser";

					using (var response = await httpClient.PostAsync(url, content))
					{
						await HandleResponseAsync(response);
					}
				}
			}
			catch (Exception ex)
			{
				await Shell.Current.DisplayAlert("Hata", $"Hata Oluştu: {ex.Message}", "Tamam");
			}
			finally
			{
				ProfileImageStream?.Dispose();
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
				Age = Age.Value,
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
				await Shell.Current.DisplayAlert("Error", $"HTTP Error: {errorContent}", "OK");
			}
		}

		public IRelayCommand LoginCommand { get; }

		private async Task NavigateToLoginPageAsync()
		{
			await Shell.Current.GoToAsync("//LoginPage");
		}


	}
}
