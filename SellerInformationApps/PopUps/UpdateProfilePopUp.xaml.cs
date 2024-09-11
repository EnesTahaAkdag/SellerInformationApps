using CommunityToolkit.Maui.Views;
using SellerInformationApps.UpdatesViewModel;

namespace SellerInformationApps.PopUps
{
	public partial class UpdateProfilePopUp : Popup
	{
		private readonly UpdateProfileViewModel _viewModel;

		public UpdateProfilePopUp(UpdateProfileViewModel viewModel)
		{
			InitializeComponent();
			_viewModel = viewModel;
			BindingContext = _viewModel;
		}

		private async void SubmitButton(object sender, EventArgs e)
		{
			if (_viewModel.UserProfile != null)
			{
				await _viewModel.WriteData(_viewModel.UserProfile);
			}
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
	}
}