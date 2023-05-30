using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokTemplate.DAL;
using PustokTemplate.Helpers;
using PustokTemplate.Models;
using PustokTemplate.ViewModels;

namespace PustokTemplate.Areas.Manage.Controllers
{
	[Authorize(Roles = "SuperAdmin, Admin")]
	[Area("manage")]
    public class BookController : Controller
    {
        private readonly PustokDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public BookController(PustokDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public IActionResult Index(int page = 1, string search = null)
        {
            var query = _context.Books
                .Include(x => x.Author).Include(x => x.Genre).Include(x=>x.Images.Where(bi=>bi.IsMain==true)).AsQueryable();

            if (search != null)
                query = query.Where(x => x.Name.Contains(search));


            ViewBag.SearchValue = search;

            return View(PaginatedList<Book>.Create(query, page, 3));
        }

        public IActionResult Create()
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Book book)
        {
            if (!ModelState.IsValid)
                return View();

            if(!_context.Authors.Any(x=>x.Id == book.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "AuthorId is not correct");
                return View();
            } 

            if (!_context.Genres.Any(x => x.Id == book.GenreId))
            {
                ModelState.AddModelError("GenreId", "GenreId is not correct");
                return View();
            }

            book.GenreId = 3;

            if(book.PosterImage == null)
            {
                ModelState.AddModelError("PosterImage", "PosterImage is required");
                return View();
            }

            if (book.HoverPoster == null)
            {
                ModelState.AddModelError("HoverPoster", "HoverPoster is required");
                return View();
            }

            Image poster = new Image
            {
                Url = FileManager.Save(_environment.WebRootPath, "uploads/books", book.PosterImage),
                IsMain = true,
                Book = book
            };

            Image hoverPoster = new Image
            {
                Url = FileManager.Save(_environment.WebRootPath, "uploads/books", book.HoverPoster),
                IsMain = false,
                Book = book
            };

            foreach (var img in book.BookImages)
            {
                Image bookImage = new Image
                {
                    Url = FileManager.Save(_environment.WebRootPath, "uploads/books", img),
                };

                book.Images.Add(bookImage);

            }

            _context.Images.Add(poster);
            _context.Images.Add(hoverPoster);
            _context.Books.Add(book);
            _context.SaveChanges(); 
           

            return RedirectToAction("Index");
        }

        public IActionResult Edit (int id)
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();

            Book book = _context.Books.FirstOrDefault(x=>x.Id == id);
            return View(book);
        }
    }
}
