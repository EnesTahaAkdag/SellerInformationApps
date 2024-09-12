using CommunityToolkit.Maui.Views;
using SellerInformationApps.UpdatesViewModel;
using SellerInformationApps.ViewModel;

namespace SellerInformationApps.PopUps
{
	public partial class UpdateProfilePopUp : Popup
	{
		private readonly UpdateProfileViewModel _viewModel;
		private readonly AddOrUpdateProfilePhotosViewModel _profilePhotosViewModel;

		public UpdateProfilePopUp(UpdateProfileViewModel viewModel, AddOrUpdateProfilePhotosViewModel profilePhotosViewModel)
		{
			InitializeComponent();
			_viewModel = viewModel;
			_profilePhotosViewModel = profilePhotosViewModel;
			BindingContext = _viewModel;
		}

		private async void SubmitButton(object sender, EventArgs e)
		{
			await _viewModel.SubmitAsync();
			Close();
		}

		private void ClosePopUpButton(object sender, EventArgs e)
		{
			Close();
		}

		private void OpenUpdateProfilePasswordPupUp(object sender, EventArgs e)
		{
			var popup = new UpdateProfilePasswordPopUp(new UpdateProfilePassword());
			Application.Current.MainPage.ShowPopup(popup);
		}

		private async void OnAddOrChangeImageClicked(object sender, EventArgs e)
		{
			string action = await Application.Current.MainPage.DisplayActionSheet("Resim Kaynaðýný Seçin", "Ýptal", null, "Galeriden Seç", "Kamera ile Çek");

			if (action == "Galeriden Seç")
			{
				await PickImageFromGalleryAsync();
			}
			else if (action == "Kamera ile Çek")
			{
				await CaptureImageWithCameraAsync();
			}
		}

		private async Task PickImageFromGalleryAsync()
		{
			try
			{
				var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
				{
					Title = "Bir fotoðraf seçin"
				});

				if (result != null)
				{
					var stream = await result.OpenReadAsync();
					await _profilePhotosViewModel.AddOrUpdateProfilePhotosAsync(stream);
				}
			}
			catch (Exception ex)
			{
				await Shell.Current.DisplayAlert("Hata", ex.Message, "Tamam");
			}
		}

		private async Task CaptureImageWithCameraAsync()
		{
			try
			{
				var result = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
				{
					Title = "Bir fotoðraf çekin"
				});

				if (result != null)
				{
					var stream = await result.OpenReadAsync();
					await _profilePhotosViewModel.AddOrUpdateProfilePhotosAsync(stream);
				}
			}
			catch (Exception ex)
			{
				await Shell.Current.DisplayAlert("Hata", ex.Message, "Tamam");
			}
		}
	}
}
