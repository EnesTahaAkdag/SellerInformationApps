using CommunityToolkit.Maui.Views;
using SellerInformationApps.UpdatesViewModel;
using System.Text.RegularExpressions;


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

		private async void SubmitPassword(object sender, EventArgs e)
		{
			await _viewModel.SubmitPasswordAsync();
			if (_viewModel.IsPasswordUpdated == true)
			{
				Close();
			}
		}

		private void ClosePopUpButton(object sender, EventArgs e)
		{
			Close();
		}

		private void NewPasswordEntry_TextChanged(object sender, TextChangedEventArgs e)
		{
			string password = e.NewTextValue;

			MinLengthRule.TextColor = password.Length >= 8 ? Colors.Green : Colors.Red;

			UppercaseRule.TextColor = Regex.IsMatch(password, @"[A-Z]") ? Colors.Green : Colors.Red;

			LowercaseRule.TextColor = Regex.IsMatch(password, @"[a-z]") ? Colors.Green : Colors.Red;

			SpecialCharRule.TextColor = Regex.IsMatch(password, @"[\W_]") ? Colors.Green : Colors.Red;

		}
	}
}
