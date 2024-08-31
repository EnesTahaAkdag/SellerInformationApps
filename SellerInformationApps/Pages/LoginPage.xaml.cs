using SellerInformationApps.ViewModel;

namespace SellerInformationApps.Pages
{
	public partial class LoginPage : ContentPage
	{
		private LoginPageViewModel viewModel;

		public LoginPage()
		{
			InitializeComponent();
			viewModel = new LoginPageViewModel();
			BindingContext = viewModel;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			viewModel.UserName = string.Empty;
			viewModel.Password = string.Empty;
		}
	}
}