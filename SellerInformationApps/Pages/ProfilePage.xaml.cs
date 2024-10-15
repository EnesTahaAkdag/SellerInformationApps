using CommunityToolkit.Maui.Views;
using SellerInformationApps.PopUps;
using SellerInformationApps.UpdatesViewModel;
using SellerInformationApps.ViewModel;
using ServiceHelper.Alerts;

namespace SellerInformationApps.Pages
{
	public partial class ProfilePage : ContentPage
	{

		public AlertsHelper alertsHelper = new AlertsHelper();

		private readonly ProfilePageViewModel _viewModel;

		public string FirstName { get; set; }

		public ProfilePage(ProfilePageViewModel viewModel)
		{
			InitializeComponent();
			_viewModel = viewModel;
			BindingContext = _viewModel;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			try
			{
				await _viewModel.AccessedAsync();

				if (!string.IsNullOrEmpty(FirstName))
				{
					_viewModel.FirstName = FirstName;
				}
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"Bir hata oluþtu: {ex.Message}", true);
			}
		}

		private void ClearData()
		{
			_viewModel.ClearProfileData();
		}

		private async void UpdateProfileAsync(object sender, EventArgs e)
		{
			try
			{
				if (_viewModel.UserProfileData != null)
				{
					var userProfileUpdate = new UpdateProfileViewModel();
					var profilePhotosViewModel = new AddOrUpdateProfilePhotosViewModel();

					await userProfileUpdate.WriteData(_viewModel.UserProfileData);

					var result = await this.ShowPopupAsync(new UpdateProfilePopUp(userProfileUpdate, profilePhotosViewModel));

					if (result != null)
					{
						await _viewModel.AccessedAsync();
					}
				}
				else
				{
					await alertsHelper.ShowSnackBar("Profil bilgileri yüklenemedi", true);
				}
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"Profil güncellenirken bir hata oluþtu: {ex.Message}", true);
			}
		}
	}
}
