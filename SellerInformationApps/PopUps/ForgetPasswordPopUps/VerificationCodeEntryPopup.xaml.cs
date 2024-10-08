using CommunityToolkit.Maui.Views;

namespace SellerInformationApps.PopUps.ForgetPasswordPopUps
{
	public partial class VerificationCodeEntryPopup : Popup
	{
		public VerificationCodeEntryPopup(UpdatesViewModel.ForgetPasswordViewModels.VerifactionCodeEntryViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = viewModel;
		}
	}
}
