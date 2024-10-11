using CommunityToolkit.Maui.Views;
using SellerInformationApps.UpdatesViewModel.ForgetPasswordViewModels;

namespace SellerInformationApps.PopUps.ForgetPasswordPopUps
{
	public partial class VerificationCodeEntryPopup : Popup
	{
		public VerificationCodeEntryPopup()
		{
			InitializeComponent();
			var viewModel = new VerificationCodeEntryViewModel(this);
			BindingContext = viewModel;
		}
	}
}
