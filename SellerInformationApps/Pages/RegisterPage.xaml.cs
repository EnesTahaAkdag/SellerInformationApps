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
			string action = await DisplayActionSheet("Resim Kaynaðýný Seç", "Ýptal", null, "Galeriden Seç", "Kamera ile Çek");

			if (action == "Galeriden Seç")
			{
				await PickImageFromGalleryAsync(viewModel);
			}
			else if (action == "Kamera ile Çek")
			{
				await CaptureImageWithCameraAsync(viewModel);
			}
		}

		private async Task PickImageFromGalleryAsync(RegisterViewModel viewModel)
		{
			var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
			{
				Title = "Bir fotoðraf seçin"
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
				Title = "Bir fotoðraf çekin"
			});

			if (result != null)
			{
				viewModel.ProfileImageStream = await result.OpenReadAsync();
			}
		}
	}
}
