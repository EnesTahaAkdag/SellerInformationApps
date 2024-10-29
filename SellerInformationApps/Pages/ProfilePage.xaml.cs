using CommunityToolkit.Maui.Views;
using SellerInformationApps.PopUps;
using SellerInformationApps.UpdatesViewModel;
using SellerInformationApps.ViewModel;
using ServiceHelper.Alerts;

namespace SellerInformationApps.Pages
{
	public partial class ProfilePage : ContentPage
	{
		private readonly AlertsHelper alertsHelper = new();
		private readonly ProfilePageViewModel _viewModel;
		private UpdateProfileViewModel updateProfile;

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
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"Bir hata oluþtu: {ex.Message}", true);
			}
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			ClearData();
		}

		private void ClearData() => _viewModel.ClearProfileData();

		private async void UpdateProfileAsync(object sender, EventArgs e)
		{
			try
			{
				if (_viewModel.UserProfileData != null)
				{
					updateProfile = new UpdateProfileViewModel(new Popup());
					await updateProfile.LoadDataAsync(_viewModel.UserProfileData);

					var result = await this.ShowPopupAsync(new UpdateProfilePopUp(updateProfile));

					if (result != null)
					{
						await _viewModel.WriteData(updateProfile.ResultData);
						_viewModel.UserProfileData = updateProfile.ResultData;
						BindingContext = _viewModel;
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
