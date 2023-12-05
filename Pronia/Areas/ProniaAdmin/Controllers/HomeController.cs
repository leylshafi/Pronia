using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Pronia.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class HomeController : Controller
    {
		[Authorize(Roles = "Admin,Moderator")]
		public IActionResult Index()
        {
            return View();
        }
    }
}
