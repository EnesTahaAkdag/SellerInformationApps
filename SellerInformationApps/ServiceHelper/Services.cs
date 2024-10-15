using SellerInformationApps.Models;

namespace SellerInformationApps.Services
{
	public class SharedDataService
	{
		// Tekil bir instance oluşturuluyor
		public static SharedDataService Instance { get; } = new SharedDataService();

		// Paylaşılacak profil bilgisi ve resim
		public UserProfileData UserProfileData { get; set; }
		public ImageSource ProfileImage { get; set; }

		// Private constructor, dışarıdan instance alınmasını engelliyor
		private SharedDataService() { }
	}
}
