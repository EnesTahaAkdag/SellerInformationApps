using System.Collections.ObjectModel;

namespace SellerInformationApps.Pages;

public partial class ChartPage : ContentPage
{
	public ObservableCollection<CategoricalData> Data { get; set; }

	public ChartPage()
	{
		InitializeComponent();
		this.Data = GetCategoricalData();

	}
	private static ObservableCollection<CategoricalData> GetCategoricalData()
	{
		var data = new ObservableCollection<CategoricalData>
		{
			new CategoricalData { Category = "Greenings", Value = 52 },
			new CategoricalData { Category = "Perfecto", Value = 19 },
			new CategoricalData { Category = "NearBy", Value = 82 },
			new CategoricalData { Category = "Family", Value = 23 },
			new CategoricalData { Category = "Fresh", Value = 56 },
		};
		return data;
	}

	public class CategoricalData
	{
		public object Category { get; set; }
		public double Value { get; set; }
	}
}