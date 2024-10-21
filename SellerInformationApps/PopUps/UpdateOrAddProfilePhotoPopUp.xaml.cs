using CommunityToolkit.Maui.Views;
using SellerInformationApps.UpdatesViewModel;
using ServiceHelper.Alerts;

namespace SellerInformationApps.PopUps
{
	public partial class UpdateOrAddProfilePhotoPopUp : Popup
	{
		private readonly AlertsHelper _alertsHelper = new();
		private readonly AddOrUpdateProfilePhotosViewModel _profilePhotosViewModel;

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

		private async void SubmitButton(object sender, EventArgs e)
		{
			if (_profilePhotosViewModel.ProfileImageBase64 == null)
			{
				await _alertsHelper.ShowSnackBar("L�tfen �nce bir foto�raf se�in.", true);
				return;
			}

			await _profilePhotosViewModel.AddOrUpdateProfilePhotosAsync();
			Close();
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
					using (MemoryStream ms = new MemoryStream())
					{
						await stream.CopyToAsync(ms);
						var photo = Convert.ToBase64String(ms.ToArray());

						// G�rseli manuel olarak g�ncelle
						ProfileImage.Source = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(photo)));

						await _profilePhotosViewModel.WriteData(photo); // Veriyi de sakla
					}
				}
				else
				{
					_profilePhotosViewModel.ProfileImageBase64 = "profilephotots.png";
				}
			}
			catch (Exception ex)
			{
				await _alertsHelper.ShowSnackBar(ex.Message, true);
			}
		}
	}
}

