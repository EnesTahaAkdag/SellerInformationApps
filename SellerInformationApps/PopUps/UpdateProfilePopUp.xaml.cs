using CommunityToolkit.Maui.Views;
using SellerInformationApps.UpdatesViewModel;
using ServiceHelper.Alerts;

namespace SellerInformationApps.PopUps
{
	public partial class UpdateProfilePopUp : Popup
	{
		private readonly AlertsHelper alertsHelper = new();
		private readonly UpdateProfileViewModel _viewModel;
		private readonly Page _mainPage;

		public UpdateProfilePopUp(UpdateProfileViewModel viewModel)
		{
			InitializeComponent();
			_viewModel = viewModel;
			BindingContext = _viewModel;
			_mainPage = Application.Current?.MainPage;
		}

		public async void SubmitButton(object sender, EventArgs e)
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(_viewModel.ProfileImageBase64))
				{
					ProfileImage.Source = ImageSource.FromStream(() =>
						new MemoryStream(Convert.FromBase64String(_viewModel.ProfileImageBase64)));
				}

				await _viewModel.SubmitAsync();

				if (_viewModel.ResultData == null)
				{
					await alertsHelper.ShowSnackBar("Profil verileri g�ncellenemedi.", true);
					return;
				}

				Close(_viewModel.ResultData);
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"G�ncelleme s�ras�nda hata olu�tu: {ex.Message}", true);
			}
		}


		public void ClosePopup(object sender, EventArgs e)
		{
			Close();
		}

		private async void OpenProfilePasswordUpdatePopup(object sender, EventArgs e)
		{
			try
			{
				var popup = new UpdateProfilePasswordPopUp(new UpdateProfilePassword());
				await _mainPage?.ShowPopupAsync(popup);
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"�ifre g�ncelleme s�ras�nda hata olu�tu: {ex.Message}", true);
			}
		}

		private async void OpenProfilePhotoUpdateOrAddPopup(object sender, EventArgs e)
		{
			try
			{
				var currentPage = Application.Current.MainPage;
				string action = await currentPage.DisplayActionSheet("Resim Kayna��n� Se�", "�ptal", null, "Galeriden Se�", "Kamera ile �ek");
				switch (action)
				{
					case "Galeriden Se�":
						await PickOrCaptureImageAsync(isPickPhoto: true); break;
					case "Kamera ile �ek":
						await PickOrCaptureImageAsync(isPickPhoto: false); break;
				}

			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"Bir hata olu�tu: {ex.Message}", true);
			}
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
					using (var stream = await result.OpenReadAsync())
					{
						using (var ms = new MemoryStream())
						{
							await stream.CopyToAsync(ms);
							_viewModel.ProfileImageBase64 = Convert.ToBase64String(ms.ToArray());
							ProfileImage.Source = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(_viewModel.ProfileImageBase64)));
						}
					}
				}
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar(ex.Message, true);
			}
		}
	}
}
