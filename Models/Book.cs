using PustokTemplate.Attributes.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PustokTemplate.Models
{
    public class Book
    {
        public int Id { get; set; }
        [MaxLength(25)]
        [Required]
        public string Name { get; set; }
        [MaxLength(500)]
        [Required]
        public string Description { get; set; }
        [Required]
        public double InitialPrice { get; set; }
        public double DiscountPercent { get; set; }
        [Required]
        public bool IsAviable { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsNew { get; set; }
        [Required]
        public int GenreId { get; set; }
        [Required]
        public int AuthorId { get; set; }
        [MaxFileSize(2097152)]
        [AllowedExtensions("image/jpeg","image/png")]
        [NotMapped]
        public IFormFile PosterImage { get; set; }
        [MaxFileSize(2097152)]
        [AllowedExtensions("image/jpeg", "image/png")]
        [NotMapped]
        public IFormFile HoverPoster { get; set; }
        [MaxFileSize(2097152)]
        [AllowedExtensions("image/jpeg", "image/png")]
        [NotMapped]
        public List<IFormFile> BookImages { get; set; } = new List<IFormFile>();

        public Author Author { get; set;}
        public Genre Genre { get; set;}
        public List<Image> Images { get; set; } = new List<Image>();
        public List<BookTag> BookTags { get; set; }
        public List<BookComment> BookComments { get; set; }
    }
}
