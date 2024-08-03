using SellerInformationApps.ViewModel;

namespace SellerInformationApps.Pages
{
	public partial class ChartPage : ContentPage
	{
		public ChartPageViewModel ChartPageView { get; set; }

		public ChartPage()
		{
			InitializeComponent();
			ChartPageView = new ChartPageViewModel();
			BindingContext = ChartPageView;
		}
	}
}
