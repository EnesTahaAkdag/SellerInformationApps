using CommunityToolkit.Maui.Views;
using SellerInformationApps.UpdatesViewModel;
using ServiceHelper.Alerts;

namespace SellerInformationApps.PopUps
{
	public partial class UpdateOrAddProfilePhotoPopUp : Popup
	{
		private readonly AlertsHelper _alertsHelper = new();
		private readonly AddOrUpdateProfilePhotosViewModel _profilePhotosViewModel;
		private readonly UpdateProfileViewModel _updateProfileViewModel;

		public UpdateOrAddProfilePhotoPopUp(AddOrUpdateProfilePhotosViewModel profilePhotosViewModel, UpdateProfileViewModel updateProfileViewModel)
		{
			InitializeComponent();
			_profilePhotosViewModel = profilePhotosViewModel;
			_updateProfileViewModel = updateProfileViewModel;
			BindingContext = _profilePhotosViewModel;
		}

		private void ClosePopUpButton(object sender, EventArgs e)
		{
			Close(_profilePhotosViewModel.resultData);
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
			if (_profilePhotosViewModel.ProfileImageBase64 == null)
			{
				_profilePhotosViewModel.ProfileImageBase64 = "profilephotots.png";
				await _profilePhotosViewModel.AddOrUpdateProfilePhotosAsync();
			}
			else
			{
				await _profilePhotosViewModel.AddOrUpdateProfilePhotosAsync();
			}

			string newPhoto = _profilePhotosViewModel.resultData;
			Close(newPhoto);

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

						ProfileImage.Source = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(photo)));

						await _profilePhotosViewModel.WriteData(photo);
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
