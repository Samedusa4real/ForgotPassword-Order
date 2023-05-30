using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PustokTemplate.Areas.Manage.Controllers
{
	[Authorize(Roles = "SuperAdmin, Admin")]
	[Area("manage")]
    public class DashBoardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
