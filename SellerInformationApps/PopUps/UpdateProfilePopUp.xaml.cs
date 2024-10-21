using CommunityToolkit.Maui.Views;
using SellerInformationApps.UpdatesViewModel;
using ServiceHelper.Alerts;

namespace SellerInformationApps.PopUps
{
	public partial class UpdateProfilePopUp : Popup
	{
		private AlertsHelper alertsHelper = new AlertsHelper();
		private readonly UpdateProfileViewModel _viewModel;
		private readonly AddOrUpdateProfilePhotosViewModel _profilePhotosViewModel;

		public UpdateProfilePopUp(UpdateProfileViewModel viewModel, AddOrUpdateProfilePhotosViewModel profilePhotosViewModel)
		{
			InitializeComponent();
			_viewModel = viewModel;
			_profilePhotosViewModel = profilePhotosViewModel;
			_viewModel._popUp = this;
			BindingContext = _viewModel;
		}

		private void OpenProfilePasswordUpdatePopup(object sender, EventArgs e)
		{
			var popup = new UpdateProfilePasswordPopUp(new UpdateProfilePassword());
			Shell.Current.ShowPopup(popup);
		}

		private async void OpenProfilePhotoUpdateOrAddPopup(object sender, EventArgs e)
		{
			try
			{
				await _profilePhotosViewModel.WriteData(_viewModel.ProfileImageBase64);
				var popup = new UpdateOrAddProfilePhotoPopUp(_profilePhotosViewModel);
				Shell.Current.ShowPopup(popup);
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"Bir hata oluþtu: {ex.Message}", true);
			}
		}
	}
}
