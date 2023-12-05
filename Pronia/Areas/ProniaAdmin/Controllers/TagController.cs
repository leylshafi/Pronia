using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.ProniaAdmin.ViewModels;
using Pronia.DAL;
using Pronia.Models;
using System.Data;

namespace Pronia.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class TagController : Controller
    {
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }
		[Authorize(Roles = "Admin,Moderator")]
		public async Task<IActionResult> Index()
        {
            var Tags = await _context.Tags.Include(t=>t.ProductTags).ToListAsync();
            return View(Tags);
        }
		[Authorize(Roles = "Admin,Moderator")]
		public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateTagVM tagVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool result = _context.Tags.Any(c => c.Name.ToLower().Trim() == tagVM.Name.ToLower().Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "This tag already exists");
                return View();
            }
            var tag = new Tag
            {
                Name = tagVM.Name,
            };
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
		[Authorize(Roles = "Admin,Moderator")]
		public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Tag tag = await _context.Tags.FirstOrDefaultAsync(c => c.Id == id);
            if (tag is null) return NotFound();
            var vm = new UpdateTagVM { 
                Name = tag.Name,
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateTagVM tagVM)
        {
            if (!ModelState.IsValid) return View();

            Tag existed = await _context.Tags.FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();
            bool result = _context.Tags.Any(c => c.Name == tagVM.Name && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "There is already such tag");
                return View();
            }

            existed.Name = tagVM.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            var existed = await _context.Tags.FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();
            _context.Tags.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
		[Authorize(Roles = "Admin,Moderator")]
		public async Task<IActionResult> Details(int id)
		{
			if (id <= 0) return BadRequest();
			var tag = await _context.Tags.Include(c => c.ProductTags).
                ThenInclude(p => p.Product).
                ThenInclude(p=>p.ProductImages).
                FirstOrDefaultAsync(s => s.Id == id);
			if (tag == null) return NotFound();

			return View(tag);
		}
	}
}
