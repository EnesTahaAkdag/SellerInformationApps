using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using SellerInformationApps.ServiceHelper;
using ServiceHelper.Alerts;
using System.Net.Mail;
using System.Text;
using System.Windows.Input;

namespace SellerInformationApps.ViewModel
{
	public partial class RegisterViewModel : ObservableObject
	{
		private readonly AlertsHelper _alertsHelper = new();

		[ObservableProperty] private string firstName;

		[ObservableProperty] private string lastName;

		[ObservableProperty] private string userName;

		[ObservableProperty] private string email;

		[ObservableProperty] private string password;

		[ObservableProperty] private string verifyPassword;

		[ObservableProperty]
		[JsonConverter(typeof(CustemDateTimeConverter))]
		private DateTime? age = DateTime.Now;

		[ObservableProperty] private string profileImageBase64;

		private DateTime _currentDate = DateTime.Now;
		public DateTime CurrentDate => _currentDate;

		public ICommand RegisterUserCommand { get; }
		public ICommand LoginCommand { get; }

		public RegisterViewModel()
		{
			LoginCommand = new AsyncRelayCommand(NavigateToLoginPageAsync);
		}

		public async Task RegisterAsync()
		{
			if (!IsFormValid(out string validationMessage))
			{
				await _alertsHelper.ShowSnackBar(validationMessage, true);
				return;
			}

			if (!ArePasswordsMatching())
			{
				await _alertsHelper.ShowSnackBar("Şifreler eşleşmiyor.", true);
				return;
			}

			try
			{
				var user = CreateUser();
				string url = "https://5462-37-130-115-91.ngrok-free.app/RegisterAndLoginApi/RegisterUser";
				var httpClient = HttpClientFactory.Create("https://5462-37-130-115-91.ngrok-free.app");
				var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

				using (var request = new HttpRequestMessage(HttpMethod.Post, url))
				{
					request.Content = content;
					using (var response = await httpClient.SendAsync(request))
					{

						var responseContent = await response.Content.ReadAsStringAsync();
						if (response.IsSuccessStatusCode)
						{
							await _alertsHelper.ShowSnackBar("Kayıt başarılı!");
							await Shell.Current.GoToAsync("//LoginPage");
						}
						else
						{
							await _alertsHelper.ShowSnackBar($"{responseContent}", true);
						}
					}
				}
			}
			catch (Exception ex)
			{
				await _alertsHelper.ShowSnackBar($"Kayıt işlemi sırasında bir hata oluştu: {ex.Message}\n{ex.StackTrace}", true);
			}
		}

		private bool IsFormValid(out string validationMessage)
		{
			if (string.IsNullOrWhiteSpace(FirstName))
			{
				validationMessage = "Ad boş olamaz.";
				return false;
			}

			if (string.IsNullOrWhiteSpace(LastName))
			{
				validationMessage = "Soyad boş olamaz.";
				return false;
			}

			if (string.IsNullOrWhiteSpace(Email))
			{

				validationMessage = "E-posta boş olamaz.";
				return false;
			}

			if (!IsValidEmail(Email))
			{
				validationMessage = "Geçerli bir e-posta adresi giriniz.";
				return false;
			}

			if (Age == null)
			{
				validationMessage = "Yaş boş olamaz.";
				return false;
			}

			validationMessage = string.Empty;
			return true;
		}

		private bool ArePasswordsMatching()
		{
			return Password == VerifyPassword;
		}

		private bool IsValidEmail(string email)
		{
			try
			{
				var addr = new MailAddress(email);
				return addr.Address == email;
			}
			catch
			{
				return false;
			}
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
				Age = Age?.ToString("yyyy-MM-dd"),
				ProfileImageBase64 = ProfileImageBase64
			};
		}

		public void ClearFormFields()
		{
			FirstName = string.Empty;
			LastName = string.Empty;
			UserName = string.Empty;
			Email = string.Empty;
			Password = string.Empty;
			VerifyPassword = string.Empty;
			Age = DateTime.Now;
			ProfileImageBase64 = "profilephotots.png";
		}

		private async Task NavigateToLoginPageAsync()
		{
			await Shell.Current.GoToAsync("//LoginPage");
		}
	}
}
