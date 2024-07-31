namespace SellerInformationApps;

public partial class MainPage : ContentPage
{
	public MainPage() => InitializeComponent();

	private async void OpenChartPage(object sender, EventArgs e) => await Navigation.PushAsync(new Pages.ChartPage());

	private async void OpenSellerInfoPage(object sender, EventArgs e) => await Navigation.PushAsync(new Pages.SellerInfosPage());
}
