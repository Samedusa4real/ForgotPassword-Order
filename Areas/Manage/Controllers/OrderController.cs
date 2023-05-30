using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PustokTemplate.DAL;
using PustokTemplate.Enums;
using PustokTemplate.Models;
using PustokTemplate.ViewModels;

namespace PustokTemplate.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class OrderController : Controller
	{
        private readonly PustokDbContext _context;

        public OrderController(PustokDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1)
		{
            var query = _context.Orders.Include(x=>x.OrderItems).AsQueryable();
            var data = PaginatedList<Order>.Create(query, page, 8);

            //var enums = Enum.GetValues(typeof(OrderStatus));
            //Dictionary<byte,string> enumsData = new Dictionary<byte, string>();

            //foreach (var item in enums)
            //{
            //    enumsData.Add((byte)item, item.ToString());
            //}

            //ViewBag.EnumStatus = enumsData;

            var enumData = from OrderStatus e in Enum.GetValues(typeof(OrderStatus))
                           select new
                           {
                               ID = (int)e,
                               Name = e.ToString()
                           };

            ViewBag.EnumList = new SelectList(enumData, "ID", "Name");


            return View(data);
		}

        public IActionResult Detail(int id)
        {
            Order order = _context.Orders.Include(x=>x.OrderItems).ThenInclude(x=>x.Book).FirstOrDefault(x=>x.Id == id);

            if (order == null)
                return View("Error");


            return View(order);
        }

        public IActionResult Accept(int id)
        {
            Order order = _context.Orders.FirstOrDefault(x => x.Id == id);

            if (order == null)
                return View("Error");

            order.Status = OrderStatus.Accepted;
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Reject(int id)
        {
            Order order = _context.Orders.FirstOrDefault(x => x.Id == id);

            if (order == null)
                return View("Error");

            order.Status = OrderStatus.Rejected;
            _context.SaveChanges();

            return RedirectToAction("index");
        }

    }
}
