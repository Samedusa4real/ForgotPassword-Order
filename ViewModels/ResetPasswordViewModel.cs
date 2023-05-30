using System.ComponentModel.DataAnnotations;

namespace PustokTemplate.ViewModels
{
	public class ResetPasswordViewModel
	{
		[Required]
		[MaxLength(100)]
		public string Email { get; set; }
		[Required]
		[MaxLength(25)]
		[DataType(DataType.Password)]
		public string Password { get; set; }
		[Required]
		[MaxLength(25)]
		[DataType(DataType.Password)]
		[Compare(nameof(Password))]
		public string PasswordConfirmed { get; set; }
		public string Token { get; set; }
	}
}
