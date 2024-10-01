using SellerInformationApps.Pages;
using ServiceHelper.Authentication;

namespace SellerInformationApps
{
	public partial class AppShell : Shell
	{
		public AppShell()
		{
			InitializeComponent();
			RegisterRoutes();
			ConfigureTabBar();

			BindingContext = Authentication.Instance;
		}

		private void RegisterRoutes()
		{
			Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
			Routing.RegisterRoute(nameof(SellerInfosPage), typeof(SellerInfosPage));
			Routing.RegisterRoute(nameof(ChartPage), typeof(ChartPage));
			Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
			Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
			Routing.RegisterRoute(nameof(UserListPage), typeof(UserListPage));
			Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
		}

		private void ConfigureTabBar()
		{
			Shell.SetTabBarBackgroundColor(this, Color.FromArgb("#333333"));
			Shell.SetTabBarTitleColor(this, Colors.White);
			Shell.SetTabBarUnselectedColor(this, Colors.LightGray);
			Shell.SetTabBarForegroundColor(this, Colors.White);
			Shell.SetTabBarDisabledColor(this, Colors.Gray);
		}
	}
}
