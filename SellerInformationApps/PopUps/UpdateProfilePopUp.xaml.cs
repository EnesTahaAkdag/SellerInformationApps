using CommunityToolkit.Maui.Views;
using SellerInformationApps.UpdatesViewModel;
using SellerInformationApps.ViewModel;

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

			LoadUserData();
		}

		private async void LoadUserData()
		{
			if (_viewModel.UserProfile != null)
			{
				await _viewModel.WriteData(_viewModel.UserProfile);
			}
		}

		private async void SubmitButton(object sender, EventArgs e)
		{
			await _viewModel.SubmitAsync();
			Close();
		}

		private async void ClosePopUpButton(object sender, EventArgs e)
		{
			Close();
		}
		private async void OpenUpdateProfilePasswordPupUp()
		{
			
		}
	}
}
