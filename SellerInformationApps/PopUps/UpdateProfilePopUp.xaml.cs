using CommunityToolkit.Maui.Views;
using SellerInformationApps.UpdatesViewModel;
using SellerInformationApps.ViewModel;

namespace SellerInformationApps.PopUps
{
	public partial class UpdateProfilePopUp : Popup
	{
		private readonly UpdateProfileViewModel _viewModel;
		private readonly AddOrUpdateProfilePhotosViewModel _profilePhotosViewModel;

		private bool isPopupOpen = false;
		public UpdateProfilePopUp(UpdateProfileViewModel viewModel, AddOrUpdateProfilePhotosViewModel profilePhotosViewModel)
		{
			InitializeComponent();
			_viewModel = viewModel;
			_profilePhotosViewModel = profilePhotosViewModel;
			BindingContext = _viewModel;
		}

		private async void SubmitButton(object sender, EventArgs e)
		{
			if (!isPopupOpen)
			{
				isPopupOpen = true;

				var button = sender as Button;
				button.IsEnabled = false;

				await _viewModel.SubmitAsync();
				Close();

				isPopupOpen = false;
				button.IsEnabled = true;
			}
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
		private void OpenUpdateProfilePasswordPupUp(object sender, EventArgs e)
		{
			if (!isPopupOpen)
			{
				isPopupOpen = true;

				var button = sender as Button;
				button.IsEnabled = false;

				var popup = new UpdateProfilePasswordPopUp(new UpdateProfilePassword());

				popup.Closed += (s, args) =>
				{
					isPopupOpen = false;
					button.IsEnabled = true;
				};

				Application.Current.MainPage.ShowPopup(popup);
			}
		}
		private void OnAddOrChangeImageClicked(object sender, EventArgs e)
		{
			if (!isPopupOpen)
			{
				isPopupOpen = true;

				var button = sender as Button;
				button.IsEnabled = false;

				var popup = new UpdateOrAddProfilePhotoPopUp(new AddOrUpdateProfilePhotosViewModel());

				popup.Closed += (s, args) =>
				{
					isPopupOpen = false;
					button.IsEnabled = true;
				};

				Application.Current.MainPage.ShowPopup(popup);
			}
		}
	}
}
