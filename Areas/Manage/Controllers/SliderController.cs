using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PustokTemplate.DAL;
using PustokTemplate.Helpers;
using PustokTemplate.Models;
using PustokTemplate.ViewModels;

namespace PustokTemplate.Areas.Manage.Controllers
{
	[Authorize(Roles = "SuperAdmin, Admin")]
	[Area("manage")]
    public class SliderController : Controller
    {
        private readonly PustokDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public SliderController(PustokDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public IActionResult Index(int page=1)
        {
            var query = _context.Sliders.OrderBy(x=>x.Order).AsQueryable();

            return View(PaginatedList<Slider>.Create(query, page, 3));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Slider slider)
        {
            if (!ModelState.IsValid)
                return View();

            if(slider.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "ImageFile is required!");
            }


            var existingSlider = _context.Sliders.FirstOrDefault(x => x.Order == slider.Order);

            if (existingSlider != null)
            {
                ModelState.AddModelError("Order", "The order must be unique.");
                return View();
            }

            var minOrder = _context.Sliders.Min(x => x.Order);

            if (slider.Order < minOrder)
            {
                var slidersToUpdate = _context.Sliders.Where(x => x.Order >= slider.Order && x.Id != existingSlider.Id).ToList();

                foreach (var item in slidersToUpdate)
                {
                    item.Order++;
                }
            }


            slider.Url = FileManager.Save(_environment.WebRootPath,"uploads/sliders",slider.ImageFile);

            _context.Sliders.Add(slider);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            Slider slider = _context.Sliders.Find(id);

            if (slider == null) return View("Error");

            return View(slider);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(Slider slider)
        {
            if (!ModelState.IsValid) return View();

            Slider existSlider = _context.Sliders.Find(slider.Id);

            if (existSlider == null) return View("Error");

            existSlider.Order = slider.Order;
            existSlider.Header = slider.Header;
            existSlider.Desc = slider.Desc;
            existSlider.Button = slider.Button;
            existSlider.Url = slider.Url;

            string oldFileName = null;
            if (slider.ImageFile != null)
            {
                oldFileName = existSlider.Url;
                existSlider.Url = FileManager.Save(_environment.WebRootPath, "uploads/sliders", slider.ImageFile);
            }

            _context.SaveChanges();

            if (oldFileName != null)
                FileManager.Delete(_environment.WebRootPath, "uploads/sliders", oldFileName);

            return RedirectToAction("Index");
        }

    }
}
