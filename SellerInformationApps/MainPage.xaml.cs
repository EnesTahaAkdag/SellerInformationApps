using SellerInformationApps.ViewModel;

namespace SellerInformationApps;

public partial class MainPage : ContentPage
{
	private readonly MainPageViewModel viewModel;


	public MainPage()
	{
		InitializeComponent();
		viewModel = new MainPageViewModel();
		BindingContext = viewModel;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		viewModel.LoadUserName();
	}

	private async void OpenChartPage(object sender, EventArgs e)
	{
		await NavigateToPage("//ChartPage");
	}

	private async void OpenSellerInfoPage(object sender, EventArgs e)
	{
		await NavigateToPage("//SellerInfosPage");
	}

	private async void OpenLoginPage(object sender, EventArgs e)
	{
		await NavigateToPage("//LoginPage");
	}

	private async void OpenRegisterPage(object sender, EventArgs e)
	{
		await NavigateToPage("//RegisterPage");
	}

	private async void OpenUserListPage(object sender, EventArgs e)
	{
		await NavigateToPage("//UserListPage");
	}

	private async void OpenProfilePage(object sender, EventArgs e)
	{
		await NavigateToPage("//ProfilePage");
	}

	private async void LogOutButton(object sender, EventArgs e)
	{
		await viewModel.LogOutAsync();
	}

	private async Task NavigateToPage(string route)
	{
		try
		{
			await Shell.Current.GoToAsync(route);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Navigation error: {ex.Message}");
		}
	}
}
