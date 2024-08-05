using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SellerInformationApps.ViewModel
{
	public class DatePickerViewModel : INotifyPropertyChanged
	{
		private DateTime _currentDate;

		public DateTime CurrentDate
		{
			get => _currentDate;
			set
			{
				_currentDate = value;
				OnPropertyChanged();
			}
		}

		public DatePickerViewModel()
		{
			CurrentDate = DateTime.Now;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
