namespace SellerInformationApps.Models
{
	public class User
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public DateTime? Age { get; set; } 
	}

	public class UserViewModel : User
	{
		public long Password { get; set; }
	}

	public class UserList : User
	{
		public long Id { get; set; }
	}

	public class UserApiResponse
	{
		public bool Success { get; set; }
		public string ErrorMessage { get; set; }
		public List<UserList> Data { get; set; }
		public int TotalCount { get; set; }
	}
}
