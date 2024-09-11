using SellerInformationApps.ViewModel;

namespace SellerInformationApps.Pages
{
	public partial class ChartPage : ContentPage
	{
		public ChartPageViewModel viewModel { get; set; }

		public ChartPage()
		{
			InitializeComponent();
			viewModel = new ChartPageViewModel();
			BindingContext = viewModel;
		}
		protected override async void OnAppearing()
		{
			base.OnAppearing();
			viewModel.Data.Clear();
			await viewModel.GetCategoricalDataAsync();
		}
	}
}
