using SellerInformationApps.Models;
using SellerInformationApps.ViewModel;

namespace SellerInformationApps.Pages
{
	[QueryProperty(nameof(UserProfileData), "UserProfileData")]

	public partial class UpdateProfilePage : ContentPage
	{
		private readonly UpdateProfileViewModel viewModel;

		public UpdateProfilePage(UpdateProfileViewModel viewModel, UserProfileData userProfileData)
		{
			InitializeComponent();
			this.viewModel = viewModel;
			BindingContext = this.viewModel;

			viewModel.UserProfile = userProfileData;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if (viewModel.UserProfile != null)
			{
				await viewModel.WriteData(viewModel.UserProfile);
			}
			else
			{
			}
		}
	}
}
