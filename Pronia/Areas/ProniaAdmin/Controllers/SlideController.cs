using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.ProjectModel;
using Pronia.DAL;
using Pronia.Models;
using Pronia.Utilities.Extentions;

namespace Pronia.Areas.ProniaAdmin.Controllers
{
	[Area("ProniaAdmin")]
	public class SlideController : Controller
	{
		private readonly AppDbContext _context;
		private readonly IWebHostEnvironment _env;

		public SlideController(AppDbContext context, IWebHostEnvironment env)
		{
			_context = context;
			_env = env;
		}

		public async Task<IActionResult> Index()
		{
			var slides = await _context.Slides.ToListAsync();
			return View(slides);
		}
		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Create(Slide slide)
		{
			if (slide.Photo is null)
			{
				ModelState.AddModelError("Photo", "Choose Photo");
				return View();
			}
			if (!slide.Photo.ValidateType())
			{
				ModelState.AddModelError("Photo", "Wrong file type");
				return View();
			}
			if (slide.Photo.ValidateSize(2 * 1024))
			{
				ModelState.AddModelError("Photo", "It shouldn't exceed 2 mb");
				return View();
			}


			slide.ImageUrl = await slide.Photo.CreateFile(_env.WebRootPath, "assets", "images", "website-images");


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

		public async Task<IActionResult> Delete(int id)
		{
			if (id <= 0) return BadRequest();
			Slide slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
			if (slide == null) return NotFound();

			slide.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
			_context.Slides.Remove(slide);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Update(int id)
		{
			if (id <= 0) return BadRequest();
			Slide slide = await _context.Slides.FirstOrDefaultAsync(c => c.Id == id);
			if (slide is null) return NotFound();

			return View(slide);
		}

		[HttpPost]
		public async Task<IActionResult> Update(int id, Slide slide)
		{
			Slide existed = await _context.Slides.FirstOrDefaultAsync(c => c.Id == id);
			if (existed is null) return NotFound();
			if (!ModelState.IsValid) return View(existed);
			bool result = _context.Slides.Any(c => c.Title == slide.Title && c.Order == slide.Order && c.Id != id);
			if (!result)
			{
				if (slide.Photo is not null)
				{
					if (!slide.Photo.ValidateType())
					{
						ModelState.AddModelError("Photo", "Wrong file type");
						return View(existed);
					}
					if (slide.Photo.ValidateSize(2 * 1024))
					{
						ModelState.AddModelError("Photo", "It shouldn't exceed 2 mb");
						return View(existed);
					}
					string newImage = await slide.Photo.CreateFile(_env.WebRootPath, "assets", "images", "website-images");
					existed.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
					existed.ImageUrl = newImage;

				}

				existed.Title = slide.Title;
				existed.Description = slide.Description;
				existed.Subtitle = slide.Subtitle;
				existed.Order = slide.Order;
				await _context.SaveChangesAsync();
			}
			else
			{
				ModelState.AddModelError("Title", "There is already such title");
				ModelState.AddModelError("Order", "There is already such order");
				return View(existed);
			}

			
			return RedirectToAction(nameof(Index));
		}
	}
}
