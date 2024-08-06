using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellerInformationApps.Models
{
	public class SellerRaitingScore
	{
		public decimal? RatingScore { get; set; }
		public Color SegmentColor { get; set; }
		public string StoreName { get; set; }
	}

	public class ApiResponses
	{
		public bool Success { get; set; }
		public string ErrorMessage { get; set; }
		public object Data { get; set; }
		public int Count { get; set; }
		public int TotalCount { get; set; }
	}
}
