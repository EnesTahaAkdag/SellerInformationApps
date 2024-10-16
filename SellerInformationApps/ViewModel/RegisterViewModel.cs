﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using PraPazar.ServiceHelper;
using SellerInformationApps.Models;
using SellerInformationApps.ServiceHelper;
using ServiceHelper.Alerts;
using System.Net.Http.Headers;
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

		private DateTime _currentDate = DateTime.Now;
		public DateTime CurrentDate => _currentDate;

		public ICommand RegisterUserCommand { get; }
		public ICommand LoginCommand { get; }

		public RegisterViewModel()
		{
			LoginCommand = new AsyncRelayCommand(NavigateToLoginPageAsync);
		}

		public async Task RegisterAsync(Stream profileImageStream)
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

				string url = "https://a8c0-37-130-115-91.ngrok-free.app/RegisterAndLoginApi/RegisterUser";
				var httpClient = HttpClientFactory.Create("https://a8c0-37-130-115-91.ngrok-free.app");

				using (var content = new MultipartFormDataContent())
				{
					if (profileImageStream != null)
					{
						var fileContent = new StreamContent(profileImageStream);
						fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
						content.Add(fileContent, "ProfileImage", "profile.jpg");
					}

					content.Add(new StringContent(UserName), "UserName");
					content.Add(new StringContent(FirstName), "FirstName");
					content.Add(new StringContent(LastName), "LastName");
					content.Add(new StringContent(Email), "Email");
					content.Add(new StringContent(Password), "Password");
					content.Add(new StringContent(Age?.ToString("yyyy-MM-dd")), "Age");

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
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"Kayıt işlemi sırasında bir hata oluştu: {ex.Message}\n{ex.StackTrace}", true);
			}
		}




		private bool IsFormValid()
		{
			if (!string.IsNullOrWhiteSpace(FirstName) &&
				!string.IsNullOrWhiteSpace(LastName) &&
				!string.IsNullOrWhiteSpace(UserName) &&
				!string.IsNullOrWhiteSpace(Email) &&
				!string.IsNullOrWhiteSpace(Password) &&
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
				Age = Age?.ToString("yyyy-MM-dd")
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
			ProfileImage = null;
		}

		private async Task NavigateToLoginPageAsync()
		{
			await Shell.Current.GoToAsync("//LoginPage");
		}
	}
}
