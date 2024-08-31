namespace SellerInformationApps;

public partial class MainPage : ContentPage
{
	public MainPage() => InitializeComponent();

	private async void OpenChartPage(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//ChartPage");
	}

	private async void OpenSellerInfoPage(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//SellerInfosPage");
	}

	private async void OpenLoginPage(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//LoginPage");
	}

	private async void OpenRegisterPage(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//RegisterPage");
	}

	private async void OpenUserListPage(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//UserListPage");
	}

	private async void OpenProfilePage(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//ProfilePage");
	}
}
