using CommunityToolkit.Mvvm.ComponentModel;
using ServiceHelper.Authentication;

namespace SellerInformationApps.ViewModel
{
	public partial class MainPageViewModel : ObservableObject
	{
		private readonly Authentication authentication;

		[ObservableProperty]
		private string usersName;

		[ObservableProperty]
		private string accessed;

		public MainPageViewModel()
		{
			authentication = Authentication.Instance;
			LoadUserName();
		}

		public void LoadUserName()
		{
			if (authentication.IsLoggedIn)
			{
				UsersName = Preferences.Get("UserName", string.Empty);
				Accessed = "Hoşgeldiniz: ";
			}
			else
			{
				Accessed = "Lütfen Giriş Yapınız";
				UsersName = string.Empty;
			}
		}
		public async Task LogOutAsync()
		{
			await Authentication.Instance.LogOut();
		}
	}
}