using SellerInformationApps.ViewModel;
using ServiceHelper.Alerts;

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
			string action = await currentPage.DisplayActionSheet("Resim Kayna��n� Se�", "�ptal", null, "Galeriden Se�", "Kamera ile �ek");

			switch (action)
			{
				case "Galeriden Se�":
					await PickOrCaptureImageAsync(isPickPhoto: true);
					break;
				case "Kamera ile �ek":
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
	}
}
