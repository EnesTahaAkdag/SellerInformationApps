using SellerInformationApps.ViewModel;

namespace SellerInformationApps.Pages
{
	public partial class SellerInfosPage : ContentPage
	{
		private bool isFetching = false;

		public SellerInfosPage(SellerInfosViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = viewModel;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			var viewModel = (SellerInfosViewModel)BindingContext;
			viewModel.CurrentPage = 1;
			await viewModel.FetchInitialDataAsync();
		}

		private async void OnScrollViewScrolled(object sender, ScrolledEventArgs e)
		{
			if (isFetching) return;

			if (sender is ScrollView scrollView)
			{
				double scrollingSpace = scrollView.ContentSize.Height - scrollView.Height;
				if (scrollingSpace <= e.ScrollY)
				{
					isFetching = true;
					var viewModel = (SellerInfosViewModel)BindingContext;
					await viewModel.FetchDataOnScrollAsync();
					isFetching = false;
				}
			}
		}

		private void OnScrolled(object sender, ScrolledEventArgs e)
		{
			headerScroll.ScrollToAsync(e.ScrollX, 0, false);
		}
	}
}
