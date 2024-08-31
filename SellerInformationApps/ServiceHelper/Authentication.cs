using CommunityToolkit.Mvvm.ComponentModel;

namespace ServiceHelper.Authentication
{
	public partial class Authentication : ObservableObject
	{
		private bool isLoggedIn;

		public bool IsLoggedIn
		{
			get => isLoggedIn;
			set
			{
				SetProperty(ref isLoggedIn, value);
				OnPropertyChanged(nameof(IsLoginVisible));
				OnPropertyChanged(nameof(IsContentVisible));
			}
		}

		public bool IsLoginVisible => !IsLoggedIn;
		public bool IsContentVisible => IsLoggedIn;

		public void LogOut()
		{
			Preferences.Remove("UserName");
			Preferences.Remove("Password");
			IsLoggedIn = false;
		}

		public void LogIn()
		{
			IsLoggedIn = true;
		}
	}
}