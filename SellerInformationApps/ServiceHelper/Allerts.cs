using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

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

		public async Task ShowToastBar(string message, bool isSuccess = false, ToastDuration duration = ToastDuration.Short)
		{

			var fontSize = isSuccess ? 16 : 14;

			var toast = Toast.Make(message, duration, fontSize);
			 
			await MainThread.InvokeOnMainThreadAsync(async () =>
			{ 
				await toast.Show();
			});
		}

	}
}
