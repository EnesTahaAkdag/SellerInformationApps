using SellerInformationApps.ViewModel;

namespace SellerInformationApps.Pages
{
	public partial class ChartPage : ContentPage
	{
		private ChartPageViewModel viewModel;

		public ChartPage()
		{
			InitializeComponent();
			viewModel = new ChartPageViewModel();
			BindingContext = viewModel;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			await viewModel.GetCategoricalDataAsync();
		}
	}
}
