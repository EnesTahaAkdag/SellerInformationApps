using SellerInformationApps.ViewModel;
using ServiceHelper.Alerts;

namespace SellerInformationApps.Pages
{
	public partial class RegisterPage : ContentPage
	{
		private AlertsHelper alertsHelper = new AlertsHelper();

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
			ResetForm();
		}

		private void ResetForm()
		{
			_registerViewModel.ClearFormFields();
		}

		private async void SubmitButton_Clicked(object sender, EventArgs e)
		{
			await _registerViewModel.RegisterAsync();
		}

		private async void SelectProfileImageButton_Clicked(object sender, EventArgs e)
		{
			string action = await DisplayActionSheet("Resim Kaynaðýný Seç", "Ýptal", null, "Galeriden Seç", "Kamera ile Çek");

			if (action == "Galeriden Seç")
			{
				await PickOrCaptureImageAsync(isPickPhoto: true);
			}
			else if (action == "Kamera ile Çek")
			{
				await PickOrCaptureImageAsync(isPickPhoto: false);
			}
		}

		private async Task PickOrCaptureImageAsync(bool isPickPhoto)
		{
			try
			{
				FileResult result;
				if (isPickPhoto)
				{
					result = await MediaPicker.PickPhotoAsync();
				}
				else
				{
					result = await MediaPicker.CapturePhotoAsync();
				}

				if (result != null)
				{
					var stream = await result.OpenReadAsync();

					using (MemoryStream ms = new MemoryStream())
					{
						await stream.CopyToAsync(ms);
						_registerViewModel.ProfileImage = ImageSource.FromStream(() => new MemoryStream(ms.ToArray()));
					}
				}
				else
				{
					_registerViewModel.ProfileImage = "profilephotots.png";
				}
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar(ex.Message, true);
			}
		}
	}
}
