using CommunityToolkit.Maui.Views;
using SellerInformationApps.UpdatesViewModel;

namespace SellerInformationApps.PopUps
{
	public partial class UpdateProfilePasswordPopUp : Popup
	{
		private readonly UpdateProfilePassword _viewModel;

		public UpdateProfilePasswordPopUp(UpdateProfilePassword viewModel)
		{
			InitializeComponent();
			_viewModel = viewModel;
			BindingContext = _viewModel;
		}

		private async void SubmitPassword(object sender,EventArgs e)
		{
			await _viewModel.SubmitPasswordAsync();
			Close();
		}

		private void ClosePopUpButton(object sender, EventArgs e)
		{
			Close();
		}
	}
}
