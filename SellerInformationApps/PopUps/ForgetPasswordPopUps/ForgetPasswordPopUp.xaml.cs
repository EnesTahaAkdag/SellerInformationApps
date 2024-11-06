using SellerInformationApps.UpdatesViewModel.ForgetPasswordViewModels;
using CommunityToolkit.Maui.Views;

namespace SellerInformationApps.PopUps.ForgetPasswordPopUps
{
	public partial class ForgetPasswordPopUp : Popup
	{
		public ForgetPasswordPopUp()
		{
			InitializeComponent();

			var viewModel = new ForgetPasswordViewModel(this);
			BindingContext = viewModel;
		}
	}
}
