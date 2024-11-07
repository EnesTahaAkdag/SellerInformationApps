using SellerInformationApps.ViewModel;

namespace SellerInformationApps.Pages
{
	public partial class UserListPage : ContentPage
	{
		private UserListViewModel viewModel;
		public UserListPage()
		{
			InitializeComponent();
			viewModel = new UserListViewModel();
			BindingContext = viewModel;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			if (viewModel.UserLists.Count == 0)
			{
				await viewModel.FetchInitialDataAsync();
			}
		}

		private async void OnScrollViewScrolled(object sender, ScrolledEventArgs e)
		{
			if (viewModel.IsLoading) return;

			if (sender is ScrollView)
			{
				await viewModel.FetchDataOnScrollAsync();
			}
		}

		private void OnScrolled(object sender, ScrolledEventArgs e)
		{
			headerScroll.ScrollToAsync(e.ScrollX, 0, false);
		}
	}
}
