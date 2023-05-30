using System.ComponentModel.DataAnnotations;

namespace PustokTemplate.ViewModels
{
	public class MemberRegisterViewModel
	{
		[Required]
		[MaxLength(25)]
		public string UserName { get; set; }
		[Required]
		[MaxLength(50)]
		public string FullName { get; set; }
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
	}
}
