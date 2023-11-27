using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.ProniaAdmin.ViewModels;
using Pronia.DAL;
using Pronia.Models;

namespace Pronia.Areas.ProniaAdmin.Controllers
{
	[Area("ProniaAdmin")]
	public class ProductController : Controller
	{
		private readonly AppDbContext _context;

		public ProductController(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			List<Product> Products = await _context.Products
				.Include(p=>p.Category)
				.Include(p=>p.ProductImages
				.Where(pi=>pi.IsPrimary==true))
				.ToListAsync();

			return View(Products);
		}

		public async Task<IActionResult> Create()
		{
			ViewBag.Categories = await _context.Categories.ToListAsync();
			ViewBag.Tags = await _context.Tags.ToListAsync();

			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Create(CreateProductVM productVM)
		{
			if (!ModelState.IsValid)
			{
                ViewBag.Categories = await _context.Categories.ToListAsync();
				ViewBag.Tags = await _context.Tags.ToListAsync();
				return View();
			}
			bool result = await _context.Categories.AnyAsync(c => c.Id == productVM.CategoryId);
			if(!result) 
			{
                ViewBag.Categories = await _context.Categories.ToListAsync();
				ViewBag.Tags = await _context.Tags.ToListAsync();
				ModelState.AddModelError("CategoryId", "There is no such category");
				return View();
			}

			foreach (int id in productVM.TagIds)
			{
				bool TagResult = await _context.Tags.AnyAsync(t => t.Id == id);
				if (!TagResult)
				{
					ViewBag.Categories = await _context.Categories.ToListAsync();
					ViewBag.Tags = await _context.Tags.ToListAsync();
					ModelState.AddModelError("TagIds", "There is no such tag");
					return View();
				}
			}
			Product product = new Product
			{
				Name = productVM.Name,
				Price = productVM.Price,
				SKU = productVM.SKU,
				CategoryId = (int)productVM.CategoryId,
				Description = productVM.Description,
				ProductTags = new(),
				ProductImages = new List<ProductImage>()
				{
					new ProductImage()
					{
						IsPrimary = true,
						Url = "1.jpg",
						Alternative = "sdvfe"
					},
					new ProductImage()
					{
						IsPrimary = false,
						Url = "1-1.jpg",
						Alternative = "sdvfe"
					},
				}
			};

			foreach (int id in productVM.TagIds)
			{
				var pTag = new ProductTag
				{
					TagId = id,
					Product = product
				};
				product.ProductTags.Add(pTag);
			}

			await _context.Products.AddAsync(product);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Details(int id)
		{
			if (id <= 0) return BadRequest();
			var product = await _context.Products
				.Include(p=>p.Category)
				.Include(p=>p.ProductTags).ThenInclude(pt=>pt.Tag)
				.Include(p=>p.ProductColors).ThenInclude(pc=>pc.Color)
				.Include(p=>p.ProductImages)
				.Include(p=>p.ProductSizes).ThenInclude(ps=>ps.Size)
				.FirstOrDefaultAsync(p => p.Id == id);
			if (product == null) return NotFound();

			return View(product);
		}

		public async Task<IActionResult> Delete(int id)
		{
			if (id <= 0) return BadRequest();
			var existed = await _context.Products.FirstOrDefaultAsync(c => c.Id == id);
			if (existed is null) return NotFound();
			_context.Products.Remove(existed);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Update(int id)
		{
			if(id<=0) return BadRequest();
			var product = await _context.Products.Include(p=>p.ProductTags).FirstOrDefaultAsync(p => p.Id == id);
			if (product == null) return NotFound();
			var vm = new UpdateProductVM
			{
				Name = product.Name,
				Description = product.Description,
				Price = product.Price,
				SKU = product.SKU,
				CategoryId = product.CategoryId,
				TagIds = product.ProductTags.Select(pt=>pt.TagId).ToList(),
				Categories = await _context.Categories.ToListAsync(),
				Tags= await _context.Tags.ToListAsync(),
			};
			return View(vm);
		}
		[HttpPost]
		public async Task<IActionResult> Update(int id,UpdateProductVM productVM)
		{
			if (!ModelState.IsValid)
			{
				productVM.Categories= await _context.Categories.ToListAsync();
				productVM.Tags = await _context.Tags.ToListAsync();
				return View(productVM);
			}

			Product existed = await _context.Products.Include(p=>p.ProductTags).FirstOrDefaultAsync(c => c.Id == id);
			if (existed is null) return NotFound();
			bool result =await _context.Products.AnyAsync(c => c.CategoryId==productVM.CategoryId);
			if (!result)
			{
				productVM.Categories = await _context.Categories.ToListAsync();
				productVM.Tags = await _context.Tags.ToListAsync();
				return View();
			}

			result = _context.Products.Any(c => c.Name == productVM.Name && c.Id != id);
			if (result)
			{
				productVM.Categories = await _context.Categories.ToListAsync();
				productVM.Tags = await _context.Tags.ToListAsync();
				ModelState.AddModelError("Name", "There is already such product");
				return View(productVM);
			}

			foreach (ProductTag pTag in existed.ProductTags)
			{
				if (!productVM.TagIds.Exists(t => t == pTag.Id))
				{
					_context.ProductTags.Remove(pTag);
				}
			}

			
			foreach (int tagId in productVM.TagIds)
			{
				if (!existed.ProductTags.Any(pt => pt.Id == tagId))
				{
					existed.ProductTags.Add(
						new()
						{
							TagId = tagId,
						});
				}
			}


			existed.Name = productVM.Name;
			existed.Description = productVM.Description;
			existed.Price = productVM.Price;
			existed.SKU = productVM.SKU;
			existed.CategoryId= productVM.CategoryId;
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
	}
}
