using Microsoft.Maui.Controls;
using SellerInformationApps.Pages;

namespace SellerInformationApps
{
	public partial class AppShell : Shell
	{
		public AppShell()
		{
			InitializeComponent();
			Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
			Routing.RegisterRoute(nameof(SellerInfosPage), typeof(SellerInfosPage));
			Routing.RegisterRoute(nameof(ChartPage), typeof(ChartPage));
			Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
			Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
			Routing.RegisterRoute(nameof(UserListPage), typeof(UserListPage));
			Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));

			// Styling Tab Bar
			Shell.SetTabBarBackgroundColor(this, Color.FromArgb("#333333"));
			Shell.SetTabBarTitleColor(this, Colors.White);
			Shell.SetTabBarUnselectedColor(this, Colors.LightGray);
		}
	}
}
