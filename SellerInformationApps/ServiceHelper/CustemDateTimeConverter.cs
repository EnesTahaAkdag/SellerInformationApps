using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellerInformationApps.ServiceHelper
{
	public class CustemDateTimeConverter : IsoDateTimeConverter
	{
		public CustemDateTimeConverter()
		{
			DateTimeFormat = "yyyy-MM-dd";
		}
	}
}
