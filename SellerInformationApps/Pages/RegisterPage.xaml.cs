using SellerInformationApps.ViewModel;

namespace SellerInformationApps.Pages
{
	public partial class RegisterPage : ContentPage
	{
		private readonly RegisterViewModel _registerViewModel;
		private Stream _profileImageStream;

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
			await _registerViewModel.RegisterAsync(_profileImageStream);
		}

		private async void SelectProfileImageButton_Clicked(object sender, EventArgs e)
		{
			string action = await DisplayActionSheet("Resim Kayna��n� Se�", "�ptal", null, "Galeriden Se�", "Kamera ile �ek");

			if (action == "Galeriden Se�")
			{
				await PickOrCaptureImageAsync(isPickPhoto: true);
			}
			else if (action == "Kamera ile �ek")
			{
				await PickOrCaptureImageAsync(isPickPhoto: false);
			}
		}

		private async Task PickOrCaptureImageAsync(bool isPickPhoto)
		{
			try
			{
				FileResult result = null;

				if (isPickPhoto)
				{
					result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions { Title = "Bir Foto�raf Se�in" });
				}
				else
				{
					result = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions { Title = "Bir Foto�raf �ekin" });
				}

				if (result != null)
				{
					_profileImageStream = await result.OpenReadAsync();
					_registerViewModel.ProfileImage = ImageSource.FromStream(() => _profileImageStream);
				}
				else
				{
					await DisplayAlert("Hata", isPickPhoto ? "Herhangi bir resim se�ilmedi." : "Foto�raf �ekilemedi.", "Tamam");
				}
			}
			catch (Exception ex)
			{
				await DisplayAlert("Hata", ex.Message, "Tamam");
			}
		}
	}
}
