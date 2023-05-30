using PustokTemplate.Models;

namespace PustokTemplate.ViewModels
{
	public class BookDetailViewModel
	{
		public Book Book { get; set; }
		public List<Book> RelatedBooks { get; set;}

		public BookComment Comment { get; set; }

	}
}
