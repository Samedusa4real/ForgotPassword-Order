using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PustokTemplate.DAL;
using PustokTemplate.Models;
using PustokTemplate.ViewModels;
using System.Security.Claims;

namespace PustokTemplate.Controllers
{
    public class BookController : Controller
    {
        private readonly PustokDbContext _context;

        public BookController(PustokDbContext context)
        {
            _context = context;
        }
		public IActionResult Detail(int id, int? loadMoreComments)
		{
			Book book = _context.Books
				.Include(x => x.Images)
				.Include(x => x.Genre)
				.Include(x => x.Author)
				.Include(x => x.BookComments).ThenInclude(x => x.AppUser)
				.Include(x => x.BookTags).ThenInclude(y => y.Tag).FirstOrDefault(x => x.Id == id);

			if (book == null)
				return StatusCode(404);

			int commentsToShow = 3;

            if (loadMoreComments.HasValue)
            {
                commentsToShow += loadMoreComments.Value;
            }

            BookDetailViewModel bdvm = new BookDetailViewModel
			{
				Book = book,
				RelatedBooks = _context.Books.Include(x => x.Images).Include(x=>x.Author).Where(x => x.GenreId == book.GenreId).ToList(),
				Comment = new BookComment { BookId = id},
            };

			ViewBag.CommentsToShow = commentsToShow;


			return View(bdvm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Comment(BookComment comment)
		{
			if (!User.Identity.IsAuthenticated || !User.IsInRole("Member"))
				return RedirectToAction("login", "account", new { returnUrl = Url.Action("detail", "book", new { id = comment.BookId }) });

            if (!ModelState.IsValid)
			{
                Book book = _context.Books
               .Include(x => x.Images)
               .Include(x => x.Genre)
               .Include(x => x.Author)
               .Include(x => x.BookComments).ThenInclude(x => x.AppUser)
               .Include(x => x.BookTags).ThenInclude(y => y.Tag).FirstOrDefault(x => x.Id == comment.BookId);

                if (book == null)
                    return StatusCode(404);

                BookDetailViewModel bdvm = new BookDetailViewModel
                {
                    Book = book,
                    RelatedBooks = _context.Books.Include(x => x.Images).Include(x => x.Author).Where(x => x.GenreId == book.GenreId).ToList(),
                    Comment = new BookComment { BookId = comment.BookId },
                };

                bdvm.Comment = comment;
                return View("Detail", bdvm);

            }

            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			comment.AppUserId = UserId;
			comment.CreatedAt = DateTime.UtcNow.AddHours(4);

			_context.BookComments.Add(comment);
			_context.SaveChanges();

			return RedirectToAction("detail", new { id = comment.BookId });
		}


        public IActionResult GetBookDetail(int id)
        {
            Book book = _context.Books
                .Include(x => x.Author)
                .Include(x => x.Images)
                .Include(x => x.BookTags).ThenInclude(x => x.Tag)
                .FirstOrDefault(x => x.Id == id);

            return PartialView("_BookModalPartial", book);
        }

        public IActionResult AddToBasket(int id)
        {
            if(User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var basketItem = _context.BasketItems.FirstOrDefault(x => x.BookId == id && x.AppUserId == userId);

                if (basketItem != null)
                    basketItem.Count++;

                else
                {
                    basketItem = new BasketItem { AppUserId = userId, BookId = id, Count = 1 };
                    _context.BasketItems.Add(basketItem);   
                }
                _context.SaveChanges();
                var basketItems = _context.BasketItems.Include(x=>x.Book).ThenInclude(x=>x.Images).Where(x => x.AppUserId == userId).ToList();

				return PartialView("_BasketPartialView", GenerateBasketVM(basketItems));
			}
            else
            {
				List<BasketItemCountViewModel> cookieItems = new List<BasketItemCountViewModel>();
				BasketItemCountViewModel cookieItem;

				var basketStr = Request.Cookies["basket"];
				if (basketStr != null)
				{
					cookieItems = JsonConvert.DeserializeObject<List<BasketItemCountViewModel>>(basketStr);

					cookieItem = cookieItems.FirstOrDefault(x => x.BookId == id);

					if (cookieItem != null)
					{
						cookieItem.Count++;
					}
					else
					{
						cookieItem = new BasketItemCountViewModel
						{
							BookId = id,
							Count = 1
						};
						cookieItems.Add(cookieItem);
					}
				}
				else
				{
					cookieItem = new BasketItemCountViewModel
					{
						BookId = id,
						Count = 1
					};
					cookieItems.Add(cookieItem);
				}
				Response.Cookies.Append("basket", JsonConvert.SerializeObject(cookieItems));

				return PartialView("_BasketPartialView", GenerateBasketVM(cookieItems));
			}

        }

        public IActionResult AddAndRemoveBasket(int id)
        {
            List<int> ids = new List<int>();

            var basketStr = Request.Cookies["basket"];
            if (basketStr != null)
            {
                ids = JsonConvert.DeserializeObject<List<int>>(basketStr);
            }

            if (ids.Contains(id))
            {
                ids.Remove(id);
            }
            else
            {
                ids.Add(id);
            }

            Response.Cookies.Append("basket", JsonConvert.SerializeObject(ids));
            return RedirectToAction("index", "home");
        }

        public IActionResult ShowBasket()
        {
            var basketStr = Request.Cookies["basket"];
            var basketVal = JsonConvert.DeserializeObject<List<BasketItemCountViewModel>>(basketStr);
            return Json(new { basketVal });
        }

        private BasketViewModel GenerateBasketVM(List<BasketItemCountViewModel> cookieItems)
        {
			BasketViewModel bv = new BasketViewModel();
			foreach (var ci in cookieItems)
			{
				BasketItemViewModel bi = new BasketItemViewModel
				{
					Count = ci.Count,
					Book = _context.Books.Include(x => x.Images).FirstOrDefault(x => x.Id == ci.BookId),
				};
				bv.BasketItems.Add(bi);
				bv.TotalPrice += (bi.Book.DiscountPercent > 0 ? (bi.Book.InitialPrice * (100 - bi.Book.DiscountPercent) / 100) : bi.Book.InitialPrice) * bi.Count;
			}

            return bv;
		}

		private BasketViewModel GenerateBasketVM(List<BasketItem> basketItems)
        {
			BasketViewModel bv = new BasketViewModel();
			foreach (var item in basketItems)
			{
				BasketItemViewModel bi = new BasketItemViewModel
				{
					Count = item.Count,
					Book = item.Book,
				};
				bv.BasketItems.Add(bi);
				bv.TotalPrice += (bi.Book.DiscountPercent > 0 ? (bi.Book.InitialPrice * (100 - bi.Book.DiscountPercent) / 100) : bi.Book.InitialPrice) * bi.Count;
			}

            return bv;
		}


		public IActionResult RemoveFromBasket(int id)
        {
            if(User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
				string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				var basketItem = _context.BasketItems.FirstOrDefault(x => x.BookId == id && x.AppUserId == userId);

				if (basketItem == null)
					return StatusCode(404);

				if (basketItem.Count > 1)
					basketItem.Count--;
				else
					_context.BasketItems.Remove(basketItem);

				_context.SaveChanges();
				var basketItems = _context.BasketItems.Include(x => x.Book).ThenInclude(x => x.Images).Where(x => x.AppUserId == userId).ToList();

				return PartialView("_BasketPartialView", GenerateBasketVM(basketItems));
			}
            else
            {
				List<BasketItemCountViewModel> cookieItems = new List<BasketItemCountViewModel>();
				BasketItemCountViewModel cookieItem;

				var basketStr = Request.Cookies["basket"];

				if (basketStr == null)
					return NotFound();

				cookieItems = JsonConvert.DeserializeObject<List<BasketItemCountViewModel>>(basketStr);

				var item = cookieItems.FirstOrDefault(x => x.BookId == id);

				if (item == null)
					return StatusCode(404);

				if (item.Count > 1)
					item.Count--;
				else
					cookieItems.Remove(item);

				Response.Cookies.Append("basket", JsonConvert.SerializeObject(cookieItems));

				BasketViewModel bv = new BasketViewModel();

				foreach (var ci in cookieItems)
				{
					BasketItemViewModel bi = new BasketItemViewModel
					{
						Count = ci.Count,
						Book = _context.Books.Include(x => x.Images).FirstOrDefault(x => x.Id == ci.BookId),
					};
					bv.BasketItems.Add(bi);
					bv.TotalPrice += (bi.Book.DiscountPercent > 0 ? (bi.Book.InitialPrice * (100 - bi.Book.DiscountPercent) / 100) : bi.Book.InitialPrice) * bi.Count;
				}

				return PartialView("_BasketPartialView", bv);
			}
        }
    }
}
