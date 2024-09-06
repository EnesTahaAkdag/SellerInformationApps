using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ServiceHelper.Authentication
{
	public partial class Authentication : ObservableObject , INotifyPropertyChanged
	{
		private static Authentication instance;
		private static readonly object lockObj = new object();

		public Authentication() { }

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

		public async Task LogOut()
		{
			Preferences.Remove("UserName");
			Preferences.Remove("Password");
			IsLoggedIn = false;
			await Shell.Current.GoToAsync("//LoginPage");
		}

		public void LogIn()
		{
			IsLoggedIn = true;
		}
	}
}
