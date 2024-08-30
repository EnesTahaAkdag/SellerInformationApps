namespace SellerInformationApps;

public partial class MainPage : ContentPage
{
	public MainPage() => InitializeComponent();

	private async void OpenChartPage(object sender, EventArgs e) => await Shell.Current.Navigation.PushAsync(new Pages.ChartPage());

	private async void OpenSellerInfoPage(object sender, EventArgs e) => await Shell.Current.Navigation.PushAsync(new Pages.SellerInfosPage());

	private async void OpenLoginPage(object sender, EventArgs e) => await Shell.Current.Navigation.PushAsync(new Pages.LoginPage());

	private async void OpenRegisterPage(object sender, EventArgs e) => await Shell.Current.Navigation.PushAsync(new Pages.RegisterPage());

	private async void OpenUserListPage(object sender, EventArgs e) => await Shell.Current.Navigation.PushAsync(new Pages.UserListPage());

	private async void OpenProfilePage(object sender, EventArgs e) => await Shell.Current.Navigation.PushAsync(new Pages.ProfilePage());
}
