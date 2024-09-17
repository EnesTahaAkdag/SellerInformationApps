using CommunityToolkit.Maui.Views;
using SellerInformationApps.UpdatesViewModel;

namespace SellerInformationApps.PopUps
{
	public partial class UpdateOrAddProfilePhotoPopUp : Popup
	{
		private readonly AddOrUpdateProfilePhotosViewModel _profilePhotosViewModel;

		public UpdateOrAddProfilePhotoPopUp(AddOrUpdateProfilePhotosViewModel profilePhotosViewModel)
		{
			InitializeComponent();
			_profilePhotosViewModel = profilePhotosViewModel;
		}

		private void ClosePopUpButton(object sender, EventArgs e)
		{
			Close();
		}

		private async void OnPickImageClicked(object sender, EventArgs e)
		{
			await PickOrCaptureImageAsync(isPickPhoto: true);
		}

		private async void OnCaptureImageClicked(object sender, EventArgs e)
		{
			await PickOrCaptureImageAsync(isPickPhoto: false);
		}

		private async Task PickOrCaptureImageAsync(bool isPickPhoto)
		{
			try
			{
				FileResult result = null;

				if (isPickPhoto)
				{
					result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
					{
						Title = "Bir fotoðraf seçin"
					});
				}
				else
				{
					result = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
					{
						Title = "Bir fotoðraf çekin"
					});
				}

				if (result != null)
				{
					var stream = await result.OpenReadAsync();
					await _profilePhotosViewModel.AddOrUpdateProfilePhotosAsync(stream);
					Close();
				}
				else
				{
					await ShowAlertAsync("Uyarý", isPickPhoto ? "Herhangi bir resim seçilmedi." : "Fotoðraf çekilemedi.");
				}
			}
			catch (Exception ex)
			{
				await ShowAlertAsync("Hata", ex.Message);
			}
		}

		private async Task ShowAlertAsync(string title, string message)
		{
			await Shell.Current.DisplayAlert(title, message, "Tamam");
		}
	}
}