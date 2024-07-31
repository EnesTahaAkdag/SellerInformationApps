using SellerInformationApps.Pages;

namespace SellerInformationApps
{
	public partial class AppShell : Shell
	{
		public AppShell()
		{
			InitializeComponent();
			Routing.RegisterRoute(nameof(MainPage),typeof(MainPage));
			Routing.RegisterRoute(nameof(SellerInfosPage), typeof(SellerInfosPage));
			Routing.RegisterRoute(nameof(ChartPage), typeof(ChartPage));
		}
	}
}
