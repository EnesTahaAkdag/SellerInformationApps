using CommunityToolkit.Maui.Views;
using SellerInformationApps.UpdatesViewModel;

namespace SellerInformationApps.PopUps.ForgetPasswordPopUps
{
	public partial class ForgetPasswordPupUp : Popup
	{
		public ForgetPasswordPupUp(ForgetPasswordViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = viewModel;
		}
	}
}
