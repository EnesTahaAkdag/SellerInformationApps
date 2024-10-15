using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Internals;
using SellerInformationApps.UpdatesViewModel;
using SellerInformationApps.ViewModel;
using ServiceHelper.Alerts;

namespace SellerInformationApps.PopUps
{
	public partial class UpdateProfilePopUp : Popup
	{
		private AlertsHelper alertsHelper = new AlertsHelper();
		private readonly ProfilePageViewModel _profilePageViewModel;
		private readonly UpdateProfileViewModel _viewModel;
		private readonly AddOrUpdateProfilePhotosViewModel _profilePhotosViewModel;

		public UpdateProfilePopUp(UpdateProfileViewModel viewModel, AddOrUpdateProfilePhotosViewModel profilePhotosViewModel)
		{
			InitializeComponent();
			_viewModel = viewModel;
			_profilePhotosViewModel = profilePhotosViewModel;
			BindingContext = _viewModel;
		}

		private async void SubmitButton(object sender, EventArgs e)
		{
			await _viewModel.SubmitAsync();
			Close();
		}

		private void ClosePopUpButton(object sender, EventArgs e)
		{
			Close();
		}

		private void OpenUpdateProfilePasswordPupUp(object sender, EventArgs e)
		{
			var popup = new UpdateProfilePasswordPopUp(new UpdateProfilePassword());
			Application.Current.MainPage.ShowPopup(popup);
		}

		private async void OnAddOrChangeImageClicked(object sender, EventArgs e)
		{
			try
			{
				if (_profilePageViewModel.UserProfilePhoto != null)
				{
					// Profil resmi eklemek ya da g�ncellemek i�in popup a��l�yor
					await _profilePhotosViewModel.WriteData(_profilePageViewModel.UserProfilePhoto);

					var popup = new UpdateOrAddProfilePhotoPopUp(_profilePhotosViewModel);

					// Popup a��ld���nda bir event handler tan�mla
					popup.Opened += (s, e) =>
					{
						// A��ld���nda yap�lacak i�lemler
						Console.WriteLine("Popup a��ld�");
					};

					Application.Current.MainPage.ShowPopup(popup);
				}
			}
			catch (Exception ex)
			{
				await alertsHelper.ShowSnackBar($"Bir hata olu�tu: {ex.Message}", true);
			}
		}

	}
}
