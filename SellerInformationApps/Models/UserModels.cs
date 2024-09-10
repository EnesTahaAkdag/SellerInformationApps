using Newtonsoft.Json;
using SellerInformationApps.ServiceHelper;
using System.ComponentModel.DataAnnotations;

namespace SellerInformationApps.Models
{

	public class User
	{
		[StringLength(50, ErrorMessage = "50 Karakterden Fazla Giriş Yapılamaz")]
		[Required(ErrorMessage = "İsim Boş Bırakılamaz")]
		public string FirstName { get; set; }

		[StringLength(50, ErrorMessage = "50 Karakterden Fazla Giriş Yapılamaz")]
		[Required(ErrorMessage = "Soyisim Boş Bırakılamaz")]
		public string LastName { get; set; }

		[StringLength(50, ErrorMessage = "50 Karakterden Fazla Giriş Yapılamaz")]
		[Required(ErrorMessage = "Kullanıcı Adı Boş Bırakılamaz")]
		public string UserName { get; set; }

		[StringLength(50, ErrorMessage = "50 Karakterden Fazla Giriş Yapılamaz")]
		[Required(ErrorMessage = "Email Boş Bırakılamaz")]
		[EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
		public string Email { get; set; }

		[JsonProperty("Age")]
		[JsonConverter(typeof(CustemDateTimeConverter))]
		public DateTime Age { get; set; }

		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Lütfen Şifre Giriniz")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])[a-zA-Z\d]{8,}$", ErrorMessage = "En az 8 karakterlik bir parola büyük ve küçük harflerin bir kombinasyonunu içermelidir.")]
		public string Password { get; set; }
	}

	public class UpdateUserPassword
	{
		[StringLength(50, ErrorMessage = "50 Karakterden Fazla Giriş Yapılamaz")]
		[Required(ErrorMessage = "Kullanıcı Adı Boş Bırakılamaz")]
		public string UserName { get; set; }

		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Lütfen Şifre Giriniz")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])[a-zA-Z\d]{8,}$", ErrorMessage = "En az 8 karakterlik bir parola büyük ve küçük harflerin bir kombinasyonunu içermelidir.")]
		public string Password { get; set; }
	}

	public class LoginUser
	{
		[StringLength(50, ErrorMessage = "50 Karakterden Fazla Giriş Yapılamaz")]
		[Required(ErrorMessage = "Kullanıcı Adı Boş Bırakılamaz")]
		public string UserName { get; set; }

		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Lütfen Şifre Giriniz")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])[a-zA-Z\d]{8,}$", ErrorMessage = "En az 8 karakterlik bir parola büyük ve küçük harflerin bir kombinasyonunu içermelidir.")]
		public string Password { get; set; }
	}

	public class UpdateUser : UserProfileData
	{

	}

	public class UserProfileData
	{
		[StringLength(50, ErrorMessage = "50 Karakterden Fazla Giriş Yapılamaz")]
		[Required(ErrorMessage = "İsim Boş Bırakılamaz")]
		public string FirstName { get; set; }

		[StringLength(50, ErrorMessage = "50 Karakterden Fazla Giriş Yapılamaz")]
		[Required(ErrorMessage = "Soyisim Boş Bırakılamaz")]
		public string LastName { get; set; }

		[StringLength(50, ErrorMessage = "50 Karakterden Fazla Giriş Yapılamaz")]
		[Required(ErrorMessage = "Kullanıcı Adı Boş Bırakılamaz")]
		public string UserName { get; set; }

		[StringLength(50, ErrorMessage = "50 Karakterden Fazla Giriş Yapılamaz")]
		[Required(ErrorMessage = "Email Boş Bırakılamaz")]
		[EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
		public string Email { get; set; }

		public DateTime? Age { get; set; }
	}

	public class UserList : User
	{
		public long Id { get; set; }
	}

	public class LoginUserViewModel : User
	{
		public long Id { get; set; }
	}

	public class ProfileApiResponse
	{
		public bool Success { get; set; }
		public string ErrorMessage { get; set; }
		public UserProfileData Data { get; set; }
	}

	public class LoginApiResponse
	{
		public bool Success { get; set; }
		public string ErrorMessage { get; set; }
		public List<LoginUser> Data { get; set; }
	}

	public class UserApiResponse
	{
		public bool Success { get; set; }
		public string ErrorMessage { get; set; }
		public List<UserList> Data { get; set; }
		public int TotalCount { get; set; }
	}

	public class UserUpdateApiResponse
	{
		public bool Success { get; set; }
		public string ErrorMessage { get; set; }
		public List<UpdateUser> Data { get; set; }
	}

	public class RegisterApiResponse
	{
		public bool Success { get; set; }
		public string ErrorMessage { get; set; }
		public List<User> Data { get; set; }
	}

	public class UpdatePasswordApiResponse
	{
		public bool Success { get; set; }
		public string ErrorMessage { get; set; }
		public UpdateUserPassword Data { get; set; }
	}
}