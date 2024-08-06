//using System.ComponentModel;
//using System.Runtime.CompilerServices;
//using System.Windows.Input;
//using SellerInformationApps.Models;

//namespace SellerInformationApps.ViewModel
//{
//	public class DataPickerViewModel : INotifyPropertyChanged
//	{
//		private string _firstName;
//		private string _lastName;
//		private string _userName;
//		private string _email;
//		private string _password;
//		private string _verifyPassword;
//		private DateTime _dateOfBirth;
//		private DatePickerViewModel _datePickerViewModel;

//		public string FirstName
//		{
//			get => _firstName;
//			set => SetProperty(ref _firstName, value);
//		}

//		public string LastName
//		{
//			get => _lastName;
//			set => SetProperty(ref _lastName, value);
//		}

//		public string UserName
//		{
//			get => _userName;
//			set => SetProperty(ref _userName, value);
//		}

//		public string Email
//		{
//			get => _email;
//			set => SetProperty(ref _email, value);
//		}

//		public DateTime DateOfBirth
//		{
//			get => _datePickerViewModel.CurrentDate;
//			set => SetProperty(ref _dateOfBirth, value);
//		}

//		public string Password
//		{
//			get => _password;
//			set => SetProperty(ref _password, value);
//		}

//		public string VerifyPassword
//		{
//			get => _verifyPassword;
//			set => SetProperty(ref _verifyPassword, value);
//		}

//		public ICommand RegisterCommand { get; }

//		public event PropertyChangedEventHandler PropertyChanged;

//		public DataPickerViewModel()
//		{
//			_datePickerViewModel = new DatePickerViewModel();
//			RegisterCommand = new Command(OnRegister);
//		}

//		protected void SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
//		{
//			if (EqualityComparer<T>.Default.Equals(backingStore, value))
//				return;

//			backingStore = value;
//			OnPropertyChanged(propertyName);
//		}

//		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
//		{
//			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//		}

//		public async void OnRegister() // Change access modifier to public
//		{
//			if (Password != VerifyPassword)
//			{
//				await Application.Current.MainPage.DisplayAlert("Hata", "Şifreler uyuşmuyor!", "Tamam");
//				return;
//			}

//			var user = new User
//			{
//				FirstName = FirstName,
//				LastName = LastName,
//				UserName = UserName,
//				Email = Email,
//				Age = DateOfBirth
//			};

//			// Veritabanına kaydet
//			var success = await RegisterPage.RegisterUser(user);

//			if (success)
//			{
//				await Application.Current.MainPage.DisplayAlert("Başarılı", "Kullanıcı kaydedildi!", "Tamam");
//			}
//			else
//			{
//				await Application.Current.MainPage.DisplayAlert("Hata", "Kullanıcı kaydedilemedi.", "Tamam");
//			}
//		}
//	}
//}
