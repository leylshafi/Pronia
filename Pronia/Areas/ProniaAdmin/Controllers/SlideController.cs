using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.ProniaAdmin.ViewModels;
using Pronia.DAL;
using Pronia.Models;
using Pronia.Utilities.Extentions;
using System.Data;

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
		[Authorize(Roles = "Admin,Moderator")]
		public async Task<IActionResult> Index()
		{
			var slides = await _context.Slides.ToListAsync();
			return View(slides);
		}
		[Authorize(Roles = "Admin,Moderator")]
		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Create(CreateSlideVm slideVM)
		{
			if(!ModelState.IsValid) return View();
			if (!slideVM.Photo.ValidateType())
			{
				ModelState.AddModelError("Photo", "Wrong file type");
				return View();
			}
			if (slideVM.Photo.ValidateSize(2 * 1024))
			{
				ModelState.AddModelError("Photo", "It shouldn't exceed 2 mb");
				return View();
			}


			string fileName = await slideVM.Photo.CreateFile(_env.WebRootPath, "assets", "images", "website-images");

			Slide slide = new Slide()
			{
				ImageUrl = fileName,
				Description = slideVM.Description,
				Order = slideVM.Order,
				Title = slideVM.Title,
				Subtitle = slideVM.Subtitle
			};

			await _context.Slides.AddAsync(slide);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
		[Authorize(Roles = "Admin,Moderator")]
		public async Task<IActionResult> Details(int id)
		{
			if (id <= 0) return BadRequest();
			var slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
			if (slide == null) return NotFound();

			return View(slide);
		}
		[Authorize(Roles = "Admin")]
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
		[Authorize(Roles = "Admin,Moderator")]
		public async Task<IActionResult> Update(int id)
		{
			if (id <= 0) return BadRequest();
			Slide slide = await _context.Slides.FirstOrDefaultAsync(c => c.Id == id);
			if (slide is null) return NotFound();

			UpdateSlideVM vm = new UpdateSlideVM()
			{
				Subtitle = slide.Subtitle,
				Title = slide.Title,
				Description = slide.Description,
				ImageUrl = slide.ImageUrl,
				Order = slide.Order
			};
			return View(vm);
		}

		[HttpPost]
		public async Task<IActionResult> Update(int id, UpdateSlideVM slidevm)
		{
			if (!ModelState.IsValid) return View(slidevm);
			Slide existed = await _context.Slides.FirstOrDefaultAsync(c => c.Id == id);
			if (existed is null) return NotFound();

			bool result = _context.Slides.Any(c => c.Title == slidevm.Title && c.Order == slidevm.Order && c.Id != id);
			if (!result)
			{
				if (slidevm.Photo is not null)
				{
					if (!slidevm.Photo.ValidateType())
					{
						ModelState.AddModelError("Photo", "Wrong file type");
						return View(slidevm);
					}
					if (slidevm.Photo.ValidateSize(2 * 1024))
					{
						ModelState.AddModelError("Photo", "It shouldn't exceed 2 mb");
						return View(slidevm);
					}
					string newImage = await slidevm.Photo.CreateFile(_env.WebRootPath, "assets", "images", "website-images");
					existed.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
					existed.ImageUrl = newImage;

				}

				existed.Title = slidevm.Title;
				existed.Description = slidevm.Description;
				existed.Subtitle = slidevm.Subtitle;
				existed.Order = slidevm.Order;
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
