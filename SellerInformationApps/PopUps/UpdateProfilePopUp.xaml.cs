using CommunityToolkit.Maui.Views;
using SellerInformationApps.Models;
using SellerInformationApps.UpdatesViewModel;
using SellerInformationApps.ViewModel;
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

		private void OpenUpdateProfilePasswordPopUp(object sender, EventArgs e)
		{
			var popup = new UpdateProfilePasswordPopUp(new UpdateProfilePassword());
			Application.Current.MainPage.ShowPopup(popup);
		}

		private async void OnAddOrChangeImageClicked(object sender, EventArgs e)
		{
			try
			{
				await _profilePhotosViewModel.WriteData(_viewModel.ProfileImage);
				var popup = new UpdateOrAddProfilePhotoPopUp(_profilePhotosViewModel);
				Application.Current.MainPage.ShowPopup(popup);
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"Bir hata oluþtu: {ex.Message}", true);
			}
		}
	}
}
