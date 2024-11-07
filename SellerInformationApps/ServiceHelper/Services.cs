using SellerInformationApps.Models;

namespace SellerInformationApps.Services
{
	public class SharedDataService
	{
		public static SharedDataService Instance { get; } = new SharedDataService();

		public UserProfileData UserProfileData { get; set; }
		public ImageSource ProfileImage { get; set; }

		private SharedDataService() { }
	}
}
