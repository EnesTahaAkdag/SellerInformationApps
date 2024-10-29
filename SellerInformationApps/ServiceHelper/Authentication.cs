using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ServiceHelper.Authentication
{
	public partial class Authentication : ObservableObject
	{
		private static Authentication instance;
		private static readonly object lockObj = new object();

		public Authentication()
		{
			CheckUserLogin();
		}

		public static Authentication Instance
		{
			get
			{
				lock (lockObj)
				{
					if (instance == null)
					{
						instance = new Authentication();
					}
					return instance;
				}
			}
		}

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

		private void CheckUserLogin()
		{
			var userName = Preferences.Get("UserName", string.Empty);
			var password = Preferences.Get("Password", string.Empty);

			if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
			{
				IsLoggedIn = true;
			}
			else
			{
				IsLoggedIn = false;
			}
		}

		public async Task LogOut()
		{
			Preferences.Remove("UserName");
			Preferences.Remove("Password");
			IsLoggedIn = false;
			await Shell.Current.GoToAsync("//MainPage");
		}

		public void LogIn()
		{
			IsLoggedIn = true;
		}
	}
}
