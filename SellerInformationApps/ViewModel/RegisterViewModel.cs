using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using SellerInformationApps.ServiceHelper;
using ServiceHelper.Alerts;
using System.Text;
using System.Windows.Input;

namespace SellerInformationApps.ViewModel
{
	public partial class RegisterViewModel : ObservableObject
	{
		public AlertsHelper alertsHelper = new AlertsHelper();

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

		[ObservableProperty]
		private ImageSource profileImage;

		public ICommand RegisterUserCommand { get; }
		public ICommand LoginCommand { get; }

		public RegisterViewModel()
		{
			LoginCommand = new AsyncRelayCommand(NavigateToLoginPageAsync);
		}

		public async Task RegisterAsync()
		{
			if (!IsFormValid())
			{
				await alertsHelper.ShowSnackBar("Lütfen tüm alanları doldurduğunuzdan emin olun.", true);
				return;
			}

			if (!ArePasswordsMatching())
			{
				await alertsHelper.ShowSnackBar("Şifreler eşleşmiyor.", true);
				return;
			}

			try
			{
				var user = CreateUser();

				string url = "https://59b7-37-130-115-91.ngrok-free.app/RegisterAndLoginApi/RegisterUser";
				var httpClient = HttpClientFactory.Create("https://59b7-37-130-115-91.ngrok-free.app");
				var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

				using (var response = await httpClient.PostAsync(url, content))
				{
					var responseContent = await response.Content.ReadAsStringAsync();

					if (response.IsSuccessStatusCode)
					{
						await alertsHelper.ShowSnackBar("Kayıt başarılı!");
						await Shell.Current.GoToAsync("/LoginPage");
					}
					else
					{
						await alertsHelper.ShowSnackBar($"Sunucu hatası: {response.StatusCode}\n{responseContent}", true);
					}
				}
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"Kayıt işlemi sırasında bir hata oluştu: {ex.Message}\n{ex.StackTrace}", true);
			}
		}

		private bool IsFormValid()
		{
			if (!string.IsNullOrWhiteSpace(FirstName) ||
				!string.IsNullOrWhiteSpace(LastName) ||
				!string.IsNullOrWhiteSpace(UserName) ||
				!string.IsNullOrWhiteSpace(Email) ||
				!string.IsNullOrWhiteSpace(Password) ||
				Age.HasValue)
			{
				try
				{
					var email = new System.Net.Mail.MailAddress(Email);
					return true;
				}
				catch
				{
					return false;
				}
			}
			return false;
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
				Age = Age?.ToString("yyyy-MM-dd"),
				ProfileImageBase64 = Convert.ToBase64String(ConvertImageSourceToBytes(ProfileImage))
			};
		}

		private byte[] ConvertImageSourceToBytes(ImageSource imageSource)
		{
			if (imageSource is StreamImageSource streamImage)
			{
				using (var stream = streamImage.Stream(CancellationToken.None).Result)
				using (MemoryStream ms = new MemoryStream())
				{
					stream.CopyTo(ms);
					return ms.ToArray();
				}
			}
			return null;
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
			ProfileImage = "profilephotots.png";
		}

		private async Task NavigateToLoginPageAsync()
		{
			await Shell.Current.GoToAsync("//LoginPage");
		}
	}
}
