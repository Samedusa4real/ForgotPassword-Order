using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PustokTemplate.DAL;
using PustokTemplate.Models;
using PustokTemplate.ViewModels;
using System.Security.Claims;

namespace PustokTemplate.Controllers
{
    public class OrderController : Controller
    {
        private readonly PustokDbContext _context;
		private readonly UserManager<AppUser> _userManager;

		public OrderController(PustokDbContext context,UserManager<AppUser> userManager)
        {
            _context = context;
			_userManager = userManager;
		}
        public async Task<IActionResult> Checkout()
        {
            OrderViewModel ovm = new OrderViewModel();
            ovm.CheckoutItems = GenerateCheckoutItems();


            if(User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
				AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

                ovm.TotalPrice = ovm.CheckoutItems.Any() ? ovm.CheckoutItems.Sum(x => x.Price * x.Count) : 0;

                ovm.Order = new OrderCreateViewModel
                {
                    Address = user.Address,
                    Email = user.Email,
                    FullName = user.FulName,
                    Phone = user.Phone,
                };

            }
                
            else
            {
				ovm.TotalPrice = ovm.CheckoutItems.Any() ? ovm.CheckoutItems.Sum(x => x.Price * x.Count) : 0;
			}

            return View(ovm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateViewModel ovm)
        {
            if(!User.Identity.IsAuthenticated || !User.IsInRole("Member"))
            {
                if (string.IsNullOrWhiteSpace(ovm.FullName))
                {
                    ModelState.AddModelError("FullName", "Fullname is required");
                }

				if (string.IsNullOrWhiteSpace(ovm.Email))
				{
					ModelState.AddModelError("Email", "Email is required");
				}   

			}

            if(!ModelState.IsValid)
            {
                OrderViewModel vm = new OrderViewModel();
                vm.CheckoutItems = GenerateCheckoutItems();
                vm.Order = ovm;

                return View("Checkout", vm);
            }

            Order order = new Order
            {
                Address = ovm.Address,
                Phone = ovm.Phone,
                Note = ovm.Note,
                Status = Enums.OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow.AddHours(4),    
            };

            var items = GenerateCheckoutItems();

            foreach (var item in items)
            {
                Book book = _context.Books.Find(item.BookId);

                OrderItem orderItem = new OrderItem
                {
                    BookId = item.BookId,
                    DiscountPrice = (decimal)book.DiscountPercent,
                    UnitCostPrice = (decimal)book.InitialPrice,
                    UnitPrice = (decimal)(book.DiscountPercent > 0 ? (book.InitialPrice * (100 - book.DiscountPercent) / 100) : book.InitialPrice),
                    Count = item.Count,
                };

                order.OrderItems.Add(orderItem);
            }

            if(User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

                order.FullName = appUser.FulName;
                order.Email = appUser.Email;
                order.AppUserId = appUser.Id;

                ClearDbBasket(appUser.Id);
            }
            else
            {

            }
            {
                order.FullName = ovm.FullName;
                order.Email = ovm.Email;

				Response.Cookies.Delete("Basket");
			}

			_context.Orders.Add(order);
            _context.SaveChanges();

            return RedirectToAction("index", "home");
        }

        private List<CheckoutItem> GenerateCheckoutItemsFromDb(string userId)
        {
            return _context.BasketItems.Include(x => x.Book).Where(x => x.AppUserId == userId).Select(x => new CheckoutItem
			{
				Count = x.Count,
				Name = x.Book.Name,
                BookId = x.BookId,
				Price = (decimal)(x.Book.DiscountPercent > 0 ? (x.Book.InitialPrice * (100 - x.Book.DiscountPercent) / 100) : x.Book.InitialPrice)
			}).ToList();
		}
		private List<CheckoutItem> GenerateCheckoutItemsFromCookie()
        {
            List<CheckoutItem> checkoutItems = new List<CheckoutItem>();

			var basketStr = Request.Cookies["basket"];

			if (basketStr != null)
			{
				List<BasketItemCountViewModel> cookieItems = JsonConvert.DeserializeObject<List<BasketItemCountViewModel>>(basketStr);

				foreach (var item in cookieItems)
				{
					Book book = _context.Books.FirstOrDefault(x => x.Id == item.BookId);
					CheckoutItem checkoutItem = new CheckoutItem
					{
						Count = item.Count,
						Name = book.Name,
                        BookId = book.Id,
						Price = (decimal)(book.DiscountPercent > 0 ? (book.InitialPrice * (100 - book.DiscountPercent) / 100) : book.InitialPrice)
					};

					checkoutItems.Add(checkoutItem);

				}
			}

            return checkoutItems;
		}

        private List<CheckoutItem> GenerateCheckoutItems()
        {
            if(User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                return GenerateCheckoutItemsFromDb(userId);
            }

            else
            {
                return GenerateCheckoutItemsFromCookie();
            }
        }

        private void ClearDbBasket(string userId)
        {
            _context.BasketItems.RemoveRange(_context.BasketItems.Where(x => x.AppUserId == userId).ToList());
            _context.SaveChanges();
        }


	}
}
