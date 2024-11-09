using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SellerInformationApps.Models
{
	public class User
	{
		[StringLength(50, ErrorMessage = "50 karakterden fazla giriş yapılamaz.")]
		[Required(ErrorMessage = "İsim boş bırakılamaz.")]
		public string FirstName { get; set; }

		[StringLength(50, ErrorMessage = "50 karakterden fazla giriş yapılamaz.")]
		[Required(ErrorMessage = "Soyisim boş bırakılamaz.")]
		public string LastName { get; set; }

		[StringLength(50, ErrorMessage = "50 karakterden fazla giriş yapılamaz.")]
		[Required(ErrorMessage = "Kullanıcı adı boş bırakılamaz.")]
		public string UserName { get; set; }

		[StringLength(50, ErrorMessage = "50 karakterden fazla giriş yapılamaz.")]
		[Required(ErrorMessage = "Email boş bırakılamaz.")]
		[EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
		public string Email { get; set; }

		[JsonProperty("Age")]
		public string Age { get; set; }

		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Lütfen şifre giriniz.")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d])[A-Za-z\d\S]{8,}$",
		   ErrorMessage = "En az 8 karakterlik bir parola, büyük ve küçük harf, rakam ve özel karakter içermelidir.")]

		public string Password { get; set; }

		public string ProfileImageBase64 { get; set; }
	}

	public class UpdateUserPassword
	{
		[StringLength(50, ErrorMessage = "50 karakterden fazla giriş yapılamaz.")]
		[Required(ErrorMessage = "Kullanıcı adı boş bırakılamaz.")]
		public string UserName { get; set; }

		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Lütfen şifre giriniz.")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d])[A-Za-z\d\S]{8,}$",
		   ErrorMessage = "En az 8 karakterlik bir parola, büyük ve küçük harf, rakam ve özel karakter içermelidir.")]

		public string Password { get; set; }
	}

	public class ProfileUpdateResult
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public DateTime Age { get; set; }
		public string ProfileImageBase64 { get; set; }
	}

	public class LoginUser
	{
		[StringLength(50, ErrorMessage = "50 karakterden fazla giriş yapılamaz.")]
		[Required(ErrorMessage = "Kullanıcı adı boş bırakılamaz.")]
		public string UserName { get; set; }

		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Lütfen şifre giriniz.")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d])[A-Za-z\d\S]{8,}$",
		   ErrorMessage = "En az 8 karakterlik bir parola, büyük ve küçük harf, rakam ve özel karakter içermelidir.")]

		public string Password { get; set; }
	}

	public class ProfilePhotoModel
	{
		public string UserName { get; set; }
		public string NewProfileImageBase64 { get; set; }
	}

	public class ProfilePohotosApiResponse
	{
		public bool Success { get; set; }
		public string ErrorMessage { get; set; }
		public ProfilePhotoModel Data { get; set; }
	}

	public class UserProfilePhoto
	{
		public string ProfileImageBase64 { get; set; }
	}

	public class UpdateUser : UserProfileData
	{
	}

	public class UserProfileData
	{
		[StringLength(50, ErrorMessage = "50 karakterden fazla giriş yapılamaz.")]
		[Required(ErrorMessage = "İsim boş bırakılamaz.")]
		public string FirstName { get; set; }

		[StringLength(50, ErrorMessage = "50 karakterden fazla giriş yapılamaz.")]
		[Required(ErrorMessage = "Soyisim boş bırakılamaz.")]
		public string LastName { get; set; }

		[StringLength(50, ErrorMessage = "50 karakterden fazla giriş yapılamaz.")]
		[Required(ErrorMessage = "Kullanıcı adı boş bırakılamaz.")]
		public string UserName { get; set; }

		[StringLength(50, ErrorMessage = "50 karakterden fazla giriş yapılamaz.")]
		[Required(ErrorMessage = "Email boş bırakılamaz.")]
		[EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
		public string Email { get; set; }

		public DateTime? Age { get; set; }

		public string ProfileImageBase64 { get; set; }
	}

	public class VerificationCodeModel
	{
		public string UserName { get; set; }
		public string ValidationCode { get; set; }
	}

	public class VerificationCodeApiResponse
	{
		public bool Success { get; set; }

		public string ErrorMessage { get; set; }

		public VerificationCodeModel Data { get; set; }
	}

	public class ChancePasswordModel
	{
		public string UserName { get; set; }
		public string Password { get; set; }
	}

	public class ChancePasswordApiResponse
	{
		public bool Success { get; set; }

		public string ErrorMessage { get; set; }

		public ChancePasswordModel Data { get; set; }
	}

	public class UserList : User
	{
		public long Id { get; set; }
	}

	public class ForgetPassword
	{
		public string UserName { get; set; }
	}

	public class ForgetPasswordApiResponse
	{
		public bool Success { get; set; }
		public string ErrorMessage { get; set; }
		public ForgetPassword Data { get; set; }
	}

	public class ProfileUpdateApiResponse
	{
		public bool Success { get; set; }
		public string ErrorMessage { get; set; }
		public UserProfileData Data { get; set; }
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
		public int TotalPage { get; set; }
	}

	public class UserUpdateApiResponse
	{
		public bool Success { get; set; }
		public string ErrorMessage { get; set; }
		public List<UpdateUser> Data { get; set; }
	}

	public class RegisterApiResponse
	{
		public string Type { get; set; }
		public string Title { get; set; }
		public int Status { get; set; }
		public Dictionary<string, List<string>> Errors { get; set; }
		public string TraceId { get; set; }
		public bool Success { get; set; }
		public string ErrorMessage { get; set; }
		public User Data { get; set; }
	}

	public class UpdatePasswordApiResponse
	{
		public bool Success { get; set; }
		public string ErrorMessage { get; set; }
		public UpdateUserPassword Data { get; set; }
	}
}
