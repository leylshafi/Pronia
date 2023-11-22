using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Models;

namespace Pronia.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class SlideController : Controller
    {
        private readonly AppDbContext _context;

        public SlideController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var slides= await _context.Slides.ToListAsync();
            return View(slides);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Slide slide)
        {
            if(slide.Photo is null)
            {
                ModelState.AddModelError("Photo", "Choose Photo");
                return View();
            }
            if (!slide.Photo.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Photo", "Wrong file type");
                return View();
            }
            if (slide.Photo.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("Photo", "It shouldn't exceed 2 mb");
                return View();
            }
            FileStream file = new FileStream(@"C:\Users\ACER\source\repos\Pronia\Pronia\wwwroot\assets\images\website-images\\" + slide.Photo.FileName,FileMode.Create);
            await slide.Photo.CopyToAsync(file);
            file.Close();
            slide.ImageUrl = slide.Photo.FileName;
           
            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();
            var slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (slide == null) return NotFound();

            return View(slide);
        }
    }
}
