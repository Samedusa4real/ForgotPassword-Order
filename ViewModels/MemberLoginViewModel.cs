using System.ComponentModel.DataAnnotations;

namespace PustokTemplate.ViewModels
{
    public class MemberLoginViewModel
    {
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(25)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
