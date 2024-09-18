using CommunityToolkit.Maui.Views;
using SellerInformationApps.UpdatesViewModel;
using System.Runtime.CompilerServices;

namespace SellerInformationApps.PopUps
{
	public partial class UpdateOrAddProfilePhotoPopUp : Popup
	{
		private readonly AddOrUpdateProfilePhotosViewModel _profilePhotosViewModel;

		private bool isPopupOpen = false;
		public UpdateOrAddProfilePhotoPopUp(AddOrUpdateProfilePhotosViewModel profilePhotosViewModel)
		{
			InitializeComponent();
			_profilePhotosViewModel = profilePhotosViewModel;
		}

		private void ClosePopUpButton(object sender, EventArgs e)
		{
			if (!isPopupOpen)
			{
				isPopupOpen = true;
				var button = sender as Button;
				button.IsEnabled = false;

				Close();

				isPopupOpen = false;
				button.IsEnabled = true;
			}
		}

		private async void OnPickImageClicked(object sender, EventArgs e)
		{
			if (!isPopupOpen)
			{
				isPopupOpen = true;
				var button = sender as Button;
				button.IsEnabled = false;

				await PickOrCaptureImageAsync(isPickPhoto: true);

				isPopupOpen = false;
				button.IsEnabled = true;
			}
		}

		private async void OnCaptureImageClicked(object sender, EventArgs e)
		{
			if (!isPopupOpen)
			{
				isPopupOpen = true;
				var button = sender as Button;
				button.IsEnabled = false;

				await PickOrCaptureImageAsync(isPickPhoto: false);

				isPopupOpen = false;
				button.IsEnabled = true;
			}
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
						Title = "Bir foto�raf se�in"
					});
				}
				else
				{
					result = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
					{
						Title = "Bir foto�raf �ekin"
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
					await ShowAlertAsync("Uyar�", isPickPhoto ? "Herhangi bir resim se�ilmedi." : "Foto�raf �ekilemedi.");
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