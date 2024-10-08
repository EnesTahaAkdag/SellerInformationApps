using CommunityToolkit.Maui.Views;

namespace SellerInformationApps.PopUps.ForgetPasswordPopUps
{
	public partial class ForgetPasswordPopUp : Popup
	{
		public ForgetPasswordPopUp()
		{
			InitializeComponent();
			var viewModel = new UpdatesViewModel.ForgetPasswordViewModels.ForgetPasswordViewModel(this); // Pop-up referansý veriliyor
			BindingContext = viewModel;
		}
	}
}
