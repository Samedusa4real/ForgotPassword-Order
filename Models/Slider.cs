using PustokTemplate.Attributes.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PustokTemplate.Models
{
    public class Slider
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Url { get; set; }
        [Required]
        [MaxLength(50)]
        public string Header { get; set; }
        [Required]
        [MaxLength(150)]
        public string Desc { get; set; }
        [MaxLength(50)]
        public string Button { get; set; }
        [Required]
        public int Order { get; set; }
        [MaxFileSize(2097050)]
        [AllowedExtensions("image/jpeg","image/png")]
        [NotMapped]
        public IFormFile ImageFile { get; set; }

    }
}
