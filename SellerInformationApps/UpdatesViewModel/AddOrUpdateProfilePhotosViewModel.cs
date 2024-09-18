using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using SellerInformationApps.Models;
using System.Net.Http.Headers;
using ServiceHelper.Authentication;
using System.Text;
using PraPazar.ServiceHelper;

namespace SellerInformationApps.UpdatesViewModel
{
	public partial class AddOrUpdateProfilePhotosViewModel : Authentication
	{
		[ObservableProperty]
		private string userName = Preferences.Get("UserName", string.Empty);

		public async Task AddOrUpdateProfilePhotosAsync(Stream imageStream)
		{

		}

		private async Task HandleResponseAsync(string responseContent)
		{
			try
			{
				var profilePhotosApiResponse = JsonConvert.DeserializeObject<ProfilePohotosApiResponse>(responseContent);

				if (profilePhotosApiResponse?.Success == true)
				{
					await Shell.Current.DisplayAlert("Başarılı", "Resminiz Güncellendi", "Tamam");
				}
				else
				{
					await Shell.Current.DisplayAlert("Hata", profilePhotosApiResponse?.ErrorMessage ?? "Bilinmeyen hata", "Tamam");
				}
			}
			catch (Exception ex)
			{
				await Shell.Current.DisplayAlert("Hata", ex.Message, "Tamam");
			}
		}
	}
}
