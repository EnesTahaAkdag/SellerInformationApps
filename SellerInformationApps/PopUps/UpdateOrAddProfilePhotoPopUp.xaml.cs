using CommunityToolkit.Maui.Views;
using SellerInformationApps.UpdatesViewModel;

namespace SellerInformationApps.PopUps
{
	public partial class UpdateOrAddProfilePhotoPopUp : Popup
	{
		private readonly AddOrUpdateProfilePhotosViewModel  _profilePhotosViewModel;
		private Stream stream;

		public UpdateOrAddProfilePhotoPopUp(AddOrUpdateProfilePhotosViewModel profilePhotosViewModel)
		{
			InitializeComponent();
			 _profilePhotosViewModel = profilePhotosViewModel;
			BindingContext = _profilePhotosViewModel;
		}

		private void ClosePopUpButton(object sender, EventArgs e)
		{
			Close();

		}

		private async void OnCaptureImageClicked(object sender, EventArgs e)
		{
			var currentPage = Application.Current.MainPage;

			string action = await currentPage.DisplayActionSheet("Resim Kayna��n� Se�", "�ptal", null, "Galeriden Se�", "Kamera ile �ek");

			if (action == "Galeriden Se�")
			{
				await PickOrCaptureImageAsync(isPickPhoto: true);
			}
			else if (action == "Kamera ile �ek")
			{
				await PickOrCaptureImageAsync(isPickPhoto: false);
			}
		}

		private async void SubmitButton(object sender, EventArgs e)
		{
			await  _profilePhotosViewModel.AddOrUpdateProfilePhotosAsync(stream);
			Close();
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
					stream = await result.OpenReadAsync();
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
