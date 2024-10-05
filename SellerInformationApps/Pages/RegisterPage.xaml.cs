using SellerInformationApps.ViewModel;

namespace SellerInformationApps.Pages
{
	public partial class RegisterPage : ContentPage
	{
		private readonly RegisterViewModel _registerViewModel;
		private Stream stream;

		public RegisterPage(RegisterViewModel registerViewModel)
		{
			InitializeComponent();
			_registerViewModel = registerViewModel;
			BindingContext = _registerViewModel;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			ResetForm();
		}

		private void ResetForm()
		{
			_registerViewModel.ClearFormFields();
		}

		private async void SubmitButton_Clicked(object sender, EventArgs e)
		{
			await _registerViewModel.RegisterAsync(stream);
		}

		private async void SelectProfileImageButton_Clicked(object sender, EventArgs e)
		{
			string action = await DisplayActionSheet("Resim Kaynaðýný Seç", "Ýptal", null, "Galeriden Seç", "Kamera ile Çek");

			if (action == "Galeriden Seç")
			{
				await PickOrCaptureImageAsync(isPickPhoto: true);
			}
			else if (action == "Kamera ile Çek")
			{
				await PickOrCaptureImageAsync(isPickPhoto: false);
			}
		}

		private async Task PickOrCaptureImageAsync(bool isPickPhoto)
		{
			try
			{
				FileResult result = null;

				if (isPickPhoto)
				{
					result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions { Title = "Bir Fotoðraf Seçin" });
				}
				else
				{
					result = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions { Title = "Bir Fotoðraf Çekin" });
				}

				if (result != null)
				{
					stream = await result.OpenReadAsync();

					using (var memoryStream = new MemoryStream())
					{
						await stream.CopyToAsync(memoryStream);

						var streamForApi = new MemoryStream(memoryStream.ToArray());
						var streamForImage = new MemoryStream(memoryStream.ToArray());

						_registerViewModel.ProfileImage = ImageSource.FromStream(() => streamForImage);

						stream = streamForApi;
					}
				}

				else
				{
					await DisplayAlert("Hata", isPickPhoto ? "Herhangi bir resim seçilmedi." : "Fotoðraf çekilemedi.", "Tamam");
				}
			}
			catch (Exception ex)
			{
				await DisplayAlert("Hata", ex.Message, "Tamam");
			}
		}
	}
}
