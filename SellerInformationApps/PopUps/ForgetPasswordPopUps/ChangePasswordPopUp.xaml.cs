using CommunityToolkit.Maui.Views;
using SellerInformationApps.UpdatesViewModel.ForgetPasswordViewModels;

namespace SellerInformationApps.PopUps.ForgetPasswordPopUps
{
	public partial class ChangePasswordPopUp : Popup
	{
		public ChangePasswordPopUp()
		{
			InitializeComponent();
			var viewModel = new ChangePasswordViewModel(this);
			BindingContext = viewModel;
		}
	}
}
