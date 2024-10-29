using System.Collections.ObjectModel;
using Newtonsoft.Json;
using SellerInformationApps.Models;
using PraPazar.ServiceHelper;
using SellerInformationApps.ViewModel;
using System.Runtime.CompilerServices;

namespace SellerInformationApps.Pages
{
	public partial class UserListPage : ContentPage
	{
		private UserListViewModel viewModel;
		private bool isFetching = false;

		public UserListPage()
		{
			InitializeComponent();
			viewModel = new UserListViewModel();
			BindingContext = viewModel;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if (viewModel.UserLists == null)
			{
				viewModel.UserLists = new ObservableCollection<UserList>();
			}
			viewModel.CurrentPage = 1;

			viewModel.UserLists.Clear();
			await viewModel.UserListDataFromAPI();
		}

		private async void OnScrollViewScrolled(object sender,ScrolledEventArgs e)
		{
			if (isFetching) return;

			if(sender is ScrollView scrollView)
			{
				isFetching = true;
				await viewModel.FetchIntialDataAsync();
				isFetching = false;
			}
		}

		private void OnScrolled(object sender, ScrolledEventArgs e)
		{
			headerScroll.ScrollToAsync(e.ScrollX, 0, false);
		}
	}

}
