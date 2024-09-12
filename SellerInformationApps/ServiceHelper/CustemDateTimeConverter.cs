using Newtonsoft.Json.Converters;

namespace SellerInformationApps.ServiceHelper
{
	public class CustemDateTimeConverter : IsoDateTimeConverter
	{
		public CustemDateTimeConverter()
		{
			DateTimeFormat = "dd-MM-yyyy";
		}
	}
}
