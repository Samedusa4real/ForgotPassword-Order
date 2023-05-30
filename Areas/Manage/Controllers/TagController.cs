using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PustokTemplate.DAL;
using PustokTemplate.Models;
using PustokTemplate.ViewModels;

namespace PustokTemplate.Areas.Manage.Controllers
{
	[Authorize(Roles = "SuperAdmin, Admin")]
	[Area("manage")]
    public class TagController : Controller
    {
        private readonly PustokDbContext _context;

        public TagController(PustokDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page=1)
        {
            var query = _context.Tags.AsQueryable();

            return View(PaginatedList<Tag>.Create(query, page, 4));
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Create(Tag tagname)
        {
            _context.Tags.Add(tagname);
            _context.SaveChanges();

            return RedirectToAction("Index");   
        }

        public IActionResult Edit (int id)
        {
            Tag tag = _context.Tags.Find(id);

            return View(tag);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Edit(Tag tagname)
        {
            if (!ModelState.IsValid)
                return View();

            Tag existTag = _context.Tags.Find(tagname.Id);

            if (existTag == null)
                return View("Error");

            if (tagname.Name != existTag.Name && _context.Tags.Any(x => x.Name == tagname.Name))
            {
                ModelState.AddModelError("Name", "This TagName already taken!");
                return View();
            }

            existTag.Name = tagname.Name;

            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Tag tag = _context.Tags.Find(id);
            if (tag == null)
                return StatusCode(404);

            _context.Remove(tag);
            _context.SaveChanges();

            return StatusCode(200);
        }
    }
}
