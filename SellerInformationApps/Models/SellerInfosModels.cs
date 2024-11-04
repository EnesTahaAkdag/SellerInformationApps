
using System.Runtime.InteropServices;

namespace SellerInformationApps.Models
{
	public class StoreInfo
	{
		public long Id { get; set; }
		public string StoreName { get; set; }
		public string Telephone { get; set; }
	}

	public class ApiResponsess
	{
		public bool Success { get; set; }
		public string ErrorMessage { get; set; }
		public List<StoreInfo> Data { get; set; }
		public int Page { get; set; }
		public int PageSize { get; set; }
		public int TotalCount { get; set; }
		public int TotalPage { get; set; }
	}

	public class StoreDetailsViewModel
	{
		public long Id { get; set; }
		public string Link { get; set; }
		public string StoreName { get; set; }
		public string Telephone { get; set; }
		public string Email { get; set; }
		public string Address { get; set; }
		public string SellerName { get; set; }
	}

	public class StoreDetailsApiResponse
	{
		public bool Success { get; set; }
		public string ErrorMessage { get; set; }
		public StoreDetailsViewModel Data { get; set; }
	}
}
