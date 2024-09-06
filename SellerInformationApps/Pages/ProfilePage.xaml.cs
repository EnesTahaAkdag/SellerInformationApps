using SellerInformationApps.ViewModel;

namespace SellerInformationApps.Pages
{
	[QueryProperty(nameof(FirstName), "FirstName")]
	public partial class ProfilePage : ContentPage
	{
		private readonly ProfilePageViewModel _viewModel;

		public string FirstName { get; set; }

		public ProfilePage()
		{
			InitializeComponent();
			_viewModel = new ProfilePageViewModel();
			BindingContext = _viewModel;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			await _viewModel.Accessed();

			// Parametreyi ViewModel'e aktar
			if (!string.IsNullOrEmpty(FirstName))
			{
				((UpdateProfileViewModel)BindingContext).FirstName = FirstName;
			}
		}
	}
}
