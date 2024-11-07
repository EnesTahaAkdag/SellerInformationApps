using CommunityToolkit.Maui.Views;
using SellerInformationApps.UpdatesViewModel;
using ServiceHelper.Alerts;

namespace SellerInformationApps.PopUps
{
	public partial class StoreDetailsPopup : Popup
	{
		private readonly AlertsHelper alertsHelper;

		public StoreDetailsPopup(SellerDetailsViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = viewModel;
			alertsHelper = new AlertsHelper();
		}

		public void ClosePopup(object sender, EventArgs e)
		{
			Close();
		}

		public async void OpenLink(object sender, EventArgs e)
		{
			if (BindingContext is SellerDetailsViewModel storeInfo && !string.IsNullOrWhiteSpace(storeInfo.Link))
			{
				if (Uri.TryCreate(storeInfo.Link, UriKind.Absolute, out Uri uri))
				{
					await Launcher.OpenAsync(uri);
				}
				else
				{
					await alertsHelper.ShowSnackBar("Geçersiz Link");
				}
			}
		}
	}
}
