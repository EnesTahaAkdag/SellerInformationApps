using CommunityToolkit.Maui.Views;
using SellerInformationApps.UpdatesViewModel.ForgetPasswordViewModels;
using System.Text.RegularExpressions;

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

		private void password_TextChanged(object sender, TextChangedEventArgs e)
		{
			string password = e.NewTextValue;

			MinLengthRule.TextColor = password.Length >= 8 ? Colors.Green : Colors.Red;

			UppercaseRule.TextColor = Regex.IsMatch(password, @"[A-Z]") ? Colors.Green : Colors.Red;

			LowercaseRule.TextColor = Regex.IsMatch(password, @"[a-z]") ? Colors.Green : Colors.Red;

			SpecialCharRule.TextColor = Regex.IsMatch(password, @"[\W_]") ? Colors.Green : Colors.Red;
		}
	}
}
