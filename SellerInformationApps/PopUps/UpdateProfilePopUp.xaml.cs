using CommunityToolkit.Maui.Views;
using SellerInformationApps.UpdatesViewModel;
using SellerInformationApps.ViewModel;

namespace SellerInformationApps.PopUps
{
	public partial class UpdateProfilePopUp : Popup
	{
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
		private void OpenUpdateProfilePasswordPupUp(object sender, EventArgs e)
		{
			var popup = new UpdateProfilePasswordPopUp(new UpdateProfilePassword());
			Application.Current.MainPage.ShowPopup(popup);
		}
		private void OnAddOrChangeImageClicked(object sender, EventArgs e)
		{
			var popup = new UpdateOrAddProfilePhotoPopUp(new AddOrUpdateProfilePhotosViewModel());
			Application.Current.MainPage.ShowPopup(popup);
		}
	}
}
