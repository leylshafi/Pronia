using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Models;

namespace Pronia.Areas.ProniaAdmin.Controllers
{
	[Area("ProniaAdmin")]
	public class SizeController : Controller
	{
		private readonly AppDbContext _context;

		public SizeController(AppDbContext context)
		{
			_context = context;
		}
		public async Task<IActionResult> Index()
		{
			var sizes = await _context.Sizes.Include(s=>s.ProductSizes).ToListAsync();
			return View(sizes);
		}

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Size size)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool result = _context.Sizes.Any(c => c.Name.ToLower().Trim() == size.Name.ToLower().Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "This size already exists");
                return View();
            }
            await _context.Sizes.AddAsync(size);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Size size = await _context.Sizes.FirstOrDefaultAsync(c => c.Id == id);
            if (size is null) return NotFound();

            return View(size);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, Size size)
        {
            if (!ModelState.IsValid) return View();

            Size existed = await _context.Sizes.FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();
            bool result = _context.Sizes.Any(c => c.Name == size.Name && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "There is already such size");
                return View();
            }

            existed.Name = size.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            var existed = await _context.Sizes.FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();
            _context.Sizes.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();
            var size = await _context.Sizes
                .Include(c => c.ProductSizes)
                .ThenInclude(pc => pc.Product)
                .ThenInclude(p => p.ProductImages).
                FirstOrDefaultAsync(s => s.Id == id);
            if (size == null) return NotFound();

            return View(size);
        }
    }
}
