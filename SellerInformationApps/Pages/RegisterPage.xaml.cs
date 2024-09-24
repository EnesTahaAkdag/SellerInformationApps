using Microsoft.Maui.Controls;
using SellerInformationApps.ViewModel;

namespace SellerInformationApps.Pages
{
	public partial class RegisterPage : ContentPage
	{
		public RegisterPage()
		{
			InitializeComponent();
			BindingContext = new RegisterViewModel();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			ResetForm();
		}

		private void ResetForm()
		{
			var viewModel = (RegisterViewModel)BindingContext;
			viewModel.FirstName = string.Empty;
			viewModel.UserName = string.Empty;
			viewModel.LastName = string.Empty;
			viewModel.Age = default;
			viewModel.Email = string.Empty;
			viewModel.Password = string.Empty;
			viewModel.VerifyPassword = string.Empty;
		}

		private async void SelectProfileImageButton(object sender, EventArgs e)
		{
			var viewModel = (RegisterViewModel)BindingContext;
			string action = await DisplayActionSheet("Resim Kayna��n� Se�", "�ptal", null, "Galeriden Se�", "Kamera ile �ek");

			if (action == "Galeriden Se�")
			{
				await PickImageFromGalleryAsync(viewModel);
			}
			else if (action == "Kamera ile �ek")
			{
				await CaptureImageWithCameraAsync(viewModel);
			}
		}

		private async Task PickImageFromGalleryAsync(RegisterViewModel viewModel)
		{
			var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
			{
				Title = "Bir foto�raf se�in"
			});

			if (result != null)
			{
				viewModel.ProfileImageStream = await result.OpenReadAsync();
			}
		}

		private async Task CaptureImageWithCameraAsync(RegisterViewModel viewModel)
		{
			var result = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
			{
				Title = "Bir foto�raf �ekin"
			});

			if (result != null)
			{
				viewModel.ProfileImageStream = await result.OpenReadAsync();
			}
		}
	}
}
