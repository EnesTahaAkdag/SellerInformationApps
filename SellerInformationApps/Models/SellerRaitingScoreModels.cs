namespace SellerInformationApps.Models
{
	namespace MarketPlaceSellerApp.ViewModel
	{
		public class SellerRatingScore
		{
			public decimal? RatingScore { get; set; }
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

}
