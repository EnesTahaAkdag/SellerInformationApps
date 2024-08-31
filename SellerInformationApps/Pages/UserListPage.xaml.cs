using System.Collections.ObjectModel;
using Newtonsoft.Json;
using SellerInformationApps.Models;
using PraPazar.ServiceHelper;
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

			if (viewModel.UserLists == null)
			{
				viewModel.UserLists = new ObservableCollection<UserList>();
			}

			viewModel.UserLists.Clear();
			await viewModel.UserListDataFromAPI();
		}

		private void OnScrolled(object sender, ScrolledEventArgs e)
		{
			headerScroll.ScrollToAsync(e.ScrollX, 0, false);
		}
	}

}
