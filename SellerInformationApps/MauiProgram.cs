using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SellerInformationApps.Models;
using SellerInformationApps.Pages;
using SellerInformationApps.PopUps;
using SellerInformationApps.UpdatesViewModel;
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
			builder.Services.AddSingleton<UpdateProfilePopUp>();
			builder.Services.AddSingleton<UserListPage>();
			builder.Services.AddSingleton<UserProfileData>();


			builder.Services.AddTransient<ChartPageViewModel>();
			builder.Services.AddTransient<LoginPageViewModel>();
			builder.Services.AddTransient<ProfilePageViewModel>();
			builder.Services.AddTransient<RegisterViewModel>();
			builder.Services.AddTransient<SellerInfosViewModel>();
			builder.Services.AddTransient<UpdateProfileViewModel>();
			builder.Services.AddTransient<UserListViewModel>();

			builder.Services.AddTransient<UpdateProfilePopUp>();
			builder.Services.AddTransient<UpdateProfileViewModel>();
			return builder.Build();
		}
	}
}
