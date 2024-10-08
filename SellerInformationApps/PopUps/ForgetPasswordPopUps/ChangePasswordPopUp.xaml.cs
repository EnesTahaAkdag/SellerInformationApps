using CommunityToolkit.Maui.Views;
using System.Numerics;
using System.Reflection;

namespace SellerInformationApps.PopUps.ForgetPasswordPopUps
{
	public partial class ChangePasswordPopUp : Popup
	{
		public ChangePasswordPopUp(UpdatesViewModel.ForgetPasswordViewModels.ChangePasswordViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = viewModel;
		}
	}
}
