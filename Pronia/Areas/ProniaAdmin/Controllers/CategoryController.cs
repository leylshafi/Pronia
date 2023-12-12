using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.ProniaAdmin.ViewModels;
using Pronia.DAL;
using Pronia.Models;

namespace Pronia.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
	public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
		[Authorize(Roles = "Admin,Moderator")]
		public async Task<IActionResult> Index(int page)
        {
            double count = await _context.Categories.CountAsync();
            List<Category> Categories = await _context.Categories.Skip(page * 2).Take(2)
                .Include(c=>c.Products).ToListAsync();
            PaginationVM<Category> pagination = new()
            {
                TotalPage = Math.Ceiling(count / 2),
                CurrentPage = page,
                Items = Categories
            };
            return View(pagination);
        }
		[Authorize(Roles = "Admin,Moderator")]
		public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryVM categoryVM)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            bool result = _context.Categories.Any(c=>c.Name.ToLower().Trim() == categoryVM.Name.ToLower().Trim());
            if(result)
            {
                ModelState.AddModelError("Name", "This category already exists");
                return View();
            }
            var category = new Category { 
                Name = categoryVM.Name,
            };

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
		[Authorize(Roles = "Admin,Moderator")]
		public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Category category = await _context.Categories.FirstOrDefaultAsync(c=>c.Id == id);
            if (category is null) return NotFound();

            var vm = new UpdateCategoryVM
            {
                Name = category.Name,
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateCategoryVM categoryvm)
        {
            if(!ModelState.IsValid) return View();

            Category existed = await _context.Categories.FirstOrDefaultAsync(c=>c.Id== id);
            if (existed is null) return NotFound();
            bool result = _context.Categories.Any(c=>c.Name==categoryvm.Name && c.Id!=id);
            if(result)
            {
                ModelState.AddModelError("Name", "There is already such category");
                return View();
            }

            existed.Name= categoryvm.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(int id)
        {
            if(id <= 0) return BadRequest();
            var existed = await _context.Categories.FirstOrDefaultAsync(c=> c.Id== id);
            if (existed is null) return NotFound();
            _context.Categories.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
		[Authorize(Roles = "Admin,Moderator")]
		public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();
            var category = await _context.Categories.Include(c=>c.Products).ThenInclude(p=>p.ProductImages).FirstOrDefaultAsync(s => s.Id == id);
            if (category == null) return NotFound();

            return View(category);
        }
    }
}
