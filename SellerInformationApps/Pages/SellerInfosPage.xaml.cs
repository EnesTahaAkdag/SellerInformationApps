using System.Collections.ObjectModel;
using SellerInformationApps.Models;
using SellerInformationApps.ViewModel;

namespace SellerInformationApps.Pages
{
	public partial class SellerInfosPage : ContentPage
	{
		public ObservableCollection<StoreInfo> StoreInfos { get; private set; }
		private SellerInfosViewModel viewModel;
		private bool isFetching = false;

		public SellerInfosPage(SellerInfosViewModel viewModel)
		{
			InitializeComponent();
			StoreInfos = new ObservableCollection<StoreInfo>();
			this.viewModel = viewModel;
			BindingContext = viewModel;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			viewModel.CurrentPage = 1;
			viewModel.StoreInfos.Clear();

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
