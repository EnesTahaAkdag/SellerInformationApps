using CommunityToolkit.Maui.Views;
using SellerInformationApps.PopUps;
using SellerInformationApps.UpdatesViewModel;
using SellerInformationApps.ViewModel;

namespace SellerInformationApps.Pages
{
	public partial class ProfilePage : ContentPage
	{
		private readonly ProfilePageViewModel _viewModel;

		public string FirstName { get; set; }
		private bool isPopupOpen = false;

		public ProfilePage(ProfilePageViewModel viewModel)
		{
			InitializeComponent();
			_viewModel = viewModel;
			BindingContext = _viewModel;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			await _viewModel.Accessed();

			if (!string.IsNullOrEmpty(FirstName))
			{
				_viewModel.FirstName = FirstName;
			}
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			ClearData();
		}

		private void ClearData()
		{
			_viewModel.ClearProfileData();
		}

		private async void UpdateProfileAsync(object sender, EventArgs e)
		{
			if (!isPopupOpen)
			{
				isPopupOpen = true;
				var button = sender as Button;
				button.IsEnabled = false;
				if (_viewModel.UserProfileData != null)
				{
					var userProfileUpdate = new UpdateProfileViewModel();
					var profilePhotosViewModel = new AddOrUpdateProfilePhotosViewModel();

					await userProfileUpdate.WriteData(_viewModel.UserProfileData);

					var result = await this.ShowPopupAsync(new UpdateProfilePopUp(userProfileUpdate, profilePhotosViewModel));

					if (result != null)
					{
						await _viewModel.Accessed();
					}
				}
				else
				{
					await Shell.Current.DisplayAlert("Hata", "Profil bilgileri yüklenemedi", "Tamam");
				}
				isPopupOpen = false;
				button.IsEnabled = true;
			}
		}
	}
}
