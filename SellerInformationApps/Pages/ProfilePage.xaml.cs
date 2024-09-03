using SellerInformationApps.ViewModel;

namespace SellerInformationApps.Pages
{
	public partial class ProfilePage : ContentPage
	{
		private readonly ProfilePageViewModel _viewModel;

		public ProfilePage()
		{
			InitializeComponent();
			_viewModel = new ProfilePageViewModel();
			BindingContext = _viewModel;
		}

		private async void OpenUpdateProfilePage(object sender, EventArgs e)
		{
			await Shell.Current.GoToAsync("//UpdateProfilePage");
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			await _viewModel.Accessed();
		}
	}
}