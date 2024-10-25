using CommunityToolkit.Maui.Views;
using SellerInformationApps.UpdatesViewModel;
using ServiceHelper.Alerts;

namespace SellerInformationApps.PopUps
{
	public partial class UpdateProfilePopUp : Popup
	{
		private readonly AlertsHelper alertsHelper = new();
		private readonly UpdateProfileViewModel _viewModel;
		private readonly AddOrUpdateProfilePhotosViewModel _profilePhotosViewModel;
		private readonly Page _mainPage;

		public UpdateProfilePopUp(UpdateProfileViewModel viewModel, AddOrUpdateProfilePhotosViewModel profilePhotosViewModel)
		{
			InitializeComponent();
			_viewModel = viewModel;
			_profilePhotosViewModel = profilePhotosViewModel;
			BindingContext = _viewModel;
			_mainPage = Application.Current?.MainPage;
		}

		public async void SubmitButton(object sender, EventArgs e)
		{
			try
			{
				await _viewModel.SubmitAsync();

				if (_viewModel.ResultData == null)
				{
					await alertsHelper.ShowSnackBar("Profil verileri güncellenemedi.", true);
					return;
				}

				Close(_viewModel.ResultData);
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"Güncelleme sýrasýnda hata oluþtu: {ex.Message}", true);
			}
		}


		public void ClosePopup(object sender,EventArgs e)
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
				await alertsHelper.ShowSnackBar($"Þifre güncelleme sýrasýnda hata oluþtu: {ex.Message}", true);
			}
		}

		private async void OpenProfilePhotoUpdateOrAddPopup(object sender, EventArgs e)
		{
			try
			{
				await _profilePhotosViewModel.WriteData(_viewModel.ProfileImageBase64);
				var popup = new UpdateOrAddProfilePhotoPopUp(_profilePhotosViewModel, _viewModel);

				var result = await _mainPage?.ShowPopupAsync(popup);
				if (result is string updatedProfileImage)
				{
					_viewModel.ProfileImageBase64 = updatedProfileImage;
					await alertsHelper.ShowSnackBar("Profil resmi baþarýyla güncellendi.", false);
				}
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"Bir hata oluþtu: {ex.Message}", true);
			}
		}
	}
}
