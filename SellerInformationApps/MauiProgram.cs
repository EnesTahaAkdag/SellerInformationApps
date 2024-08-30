using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using SellerInformationApps.Pages;
using SellerInformationApps.ViewModel;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Telerik.Maui.Controls.Compatibility;

namespace SellerInformationApps
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.UseTelerik()
				.UseSkiaSharp()
				.UseMauiCommunityToolkit()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				});

#if DEBUG
			builder.Logging.AddDebug();
#endif


			builder.Services.AddSingleton<ChartPage>();
			builder.Services.AddSingleton<LoginPage>();
			builder.Services.AddSingleton<ProfilePage>();
			builder.Services.AddSingleton<RegisterPage>();
			builder.Services.AddSingleton<SellerInfosPage>();
			builder.Services.AddSingleton<UpdateProfilePage>();
			builder.Services.AddSingleton<UserListPage>();

			builder.Services.AddSingleton<ChartPageViewModel>();
			builder.Services.AddSingleton<LoginPageViewModel>();
			builder.Services.AddSingleton<ProfilePageViewModel>();
			builder.Services.AddSingleton<RegisterViewModel>();
			builder.Services.AddSingleton<SellerInfosViewModel>();
			builder.Services.AddSingleton<UpdateProfileViewModel>();
			builder.Services.AddSingleton<UserListViewModel>();

			return builder.Build();
		}
	}
}
