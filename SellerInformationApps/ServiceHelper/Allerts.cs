using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System;
using System.Threading.Tasks;

namespace ServiceHelper.Alerts
{
	public class AlertsHelper
	{
		public async Task ShowSnackBar(string message, bool isError = false)
		{
			var backgroundColor = isError ? Colors.Red : Colors.Green;
			var textColor = Colors.White;

			var snackbar = Snackbar.Make(
				message,
				actionButtonText: "Tamam",
				duration: TimeSpan.FromSeconds(3),
				visualOptions: new SnackbarOptions
				{
					BackgroundColor = backgroundColor,
					TextColor = textColor
				});

			await snackbar.Show();
		}
	}
}
