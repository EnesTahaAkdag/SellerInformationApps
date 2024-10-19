using CommunityToolkit.Maui.Views;
using SellerInformationApps.UpdatesViewModel;
using ServiceHelper.Alerts;

namespace SellerInformationApps.PopUps
{
	public partial class UpdateOrAddProfilePhotoPopUp : Popup
	{
		private readonly AlertsHelper _alertsHelper = new AlertsHelper();
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
			string action = await currentPage.DisplayActionSheet("Resim Kaynaðýný Seç", "Ýptal", null, "Galeriden Seç", "Kamera ile Çek");

			switch (action)
			{
				case "Galeriden Seç":
					await PickOrCaptureImageAsync(isPickPhoto: true);
					break;
				case "Kamera ile Çek":
					await PickOrCaptureImageAsync(isPickPhoto: false);
					break;
			}
		}

		private async void SubmitButton(object sender, EventArgs e)
		{
			if (_profilePhotosViewModel.ProfileImage == null)
			{
				await _alertsHelper.ShowSnackBar("Lütfen önce bir fotoðraf seçin.", true);
				return;
			}

			await _profilePhotosViewModel.AddOrUpdateProfilePhotosAsync();
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
					using (var stream = await result.OpenReadAsync())
					using (MemoryStream ms = new MemoryStream())
					{
						await stream.CopyToAsync(ms);
						_profilePhotosViewModel.ProfileImage = ImageSource.FromStream(() => new MemoryStream(ms.ToArray()));
					}
				}
				else
				{
					_profilePhotosViewModel.ProfileImage = "profilephotots.png";
				}
			}
			catch (Exception ex)
			{
				await _alertsHelper.ShowSnackBar(ex.Message, true);
			}
		}
	}
}

