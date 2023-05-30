using System.ComponentModel.DataAnnotations;

namespace PustokTemplate.ViewModels
{
	public class OrderCreateViewModel
	{
		[Required]
		[MaxLength(25)]
		public string FullName { get; set; }
		[MaxLength(25)]
		public string Phone { get; set; }
		[MaxLength(250)]
		public string Address { get; set; }
		[Required]
		[MaxLength(50)]
		public string Email { get; set; }
		[MaxLength(500)]
		public string Note { get; set; }
	}
}
