using CommunityToolkit.Mvvm.ComponentModel;
using ServiceHelper.Authentication;
using System.Threading.Tasks;

namespace SellerInformationApps.ViewModel
{
	public partial class MainPageViewModel : ObservableObject
	{
		public Authentication AuthService => Authentication.Instance;

		[ObservableProperty]
		private string usersName;

		[ObservableProperty]
		private string accessed;

		public MainPageViewModel()
		{
			LoadUserName();
		}

		public void LoadUserName()
		{
			if (AuthService.IsLoggedIn)
			{
				UsersName = Preferences.Get("UserName", string.Empty);
				Accessed = "Hoşgeldin: ";
			}
			else
			{
				Accessed = "Lütfen Giriş Yapınız";
				UsersName = string.Empty;
			}
		}

		public async Task LogOutAsync()
		{
			await AuthService.LogOut();
		}
	}
}
