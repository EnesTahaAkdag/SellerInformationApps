using SellerInformationApps.ViewModel;
using ServiceHelper.Alerts;
using System.Text.RegularExpressions;

namespace SellerInformationApps.Pages
{
	public partial class RegisterPage : ContentPage
	{
		private readonly AlertsHelper _alertsHelper = new();
		private readonly RegisterViewModel _registerViewModel;

		public RegisterPage(RegisterViewModel registerViewModel)
		{
			InitializeComponent();
			_registerViewModel = registerViewModel;
			BindingContext = _registerViewModel;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			_registerViewModel.ClearFormFields();
		}

		private async void SubmitButton_Clicked(object sender, EventArgs e)
		{
			await _registerViewModel.RegisterAsync();
		}

		private async void SelectProfileImageButton_Clicked(object sender, EventArgs e)
		{
			var currentPage = Application.Current.MainPage;
			string action = await currentPage.DisplayActionSheet("Resim Kaynaðýný Seç", "Ýptal", null, "Galeriden Seç", "Kamera ile Çek");

			switch (action)
			{
				case "Galeriden Seç":
					await PickOrCaptureImageAsync(isPickPhoto: true);
					break;
				case "Kamera ile Çek":
					await PickOrCaptureImageAsync(isPickPhoto: false);
					break;
			}
		}

		private async Task PickOrCaptureImageAsync(bool isPickPhoto)
		{
			try
			{
				FileResult result = isPickPhoto
					? await MediaPicker.PickPhotoAsync()
					: await MediaPicker.CapturePhotoAsync();

				if (result != null)
				{
					var stream = await result.OpenReadAsync();
					using (MemoryStream ms = new())
					{
						await stream.CopyToAsync(ms);
						_registerViewModel.ProfileImageBase64 = Convert.ToBase64String(ms.ToArray());
					}
				}
				else
				{
					_registerViewModel.ProfileImageBase64 = "profilephotots.png";
				}
			}
			catch (Exception ex)
			{
				await _alertsHelper.ShowSnackBar(ex.Message, true);
			}
		}


		private void Entry_TextChanged(object sender, TextChangedEventArgs e)
		{
			string password = e.NewTextValue;

			// Kural 1: En az 8 karakter
			MinLengthRule.TextColor = password.Length >= 8 ? Colors.Green : Colors.Red;

			// Kural 2: En az bir büyük harf (A-Z)
			UppercaseRule.TextColor = Regex.IsMatch(password, @"[A-Z]") ? Colors.Green : Colors.Red;

			// Kural 3: En az bir küçük harf (a-z)
			LowercaseRule.TextColor = Regex.IsMatch(password, @"[a-z]") ? Colors.Green : Colors.Red;

			// Kural 4: En az bir özel karakter içermeli
			SpecialCharRule.TextColor = Regex.IsMatch(password, @"[\W_]") ? Colors.Green : Colors.Red;
		}
	}
}
