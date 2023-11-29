using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using Pronia.Areas.ProniaAdmin.ViewModels;
using Pronia.DAL;
using Pronia.Models;
using Pronia.Utilities.Extentions;

namespace Pronia.Areas.ProniaAdmin.Controllers
{
	[Area("ProniaAdmin")]
	public class ProductController : Controller
	{
		private readonly AppDbContext _context;
		private readonly IWebHostEnvironment _env;

		public ProductController(AppDbContext context,IWebHostEnvironment env)
		{
			_context = context;
			_env = env;
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
			var vm = new CreateProductVM
			{
				Categories = await _context.Categories.ToListAsync(),
				Tags = await _context.Tags.ToListAsync(),
				Colors = await _context.Colors.ToListAsync(),
				Sizes = await _context.Sizes.ToListAsync(),
			};

			return View(vm);
		}
		[HttpPost]
		public async Task<IActionResult> Create(CreateProductVM productVM)
		{
			if (!ModelState.IsValid)
			{
				productVM.Categories = await _context.Categories.ToListAsync();
				productVM.Tags = await _context.Tags.ToListAsync();
				productVM.Colors = await _context.Colors.ToListAsync();
				productVM.Sizes = await _context.Sizes.ToListAsync();
				return View(productVM);
			}
			bool result = await _context.Categories.AnyAsync(c => c.Id == productVM.CategoryId);
			if(!result) 
			{
				productVM.Categories = await _context.Categories.ToListAsync();
				productVM.Tags = await _context.Tags.ToListAsync();
				productVM.Colors = await _context.Colors.ToListAsync();
				productVM.Sizes = await _context.Sizes.ToListAsync();
				ModelState.AddModelError("CategoryId", "There is no such category");
				return View(productVM);
			}

			foreach (int id in productVM.TagIds)
			{
				bool TagResult = await _context.Tags.AnyAsync(t => t.Id == id);
				if (!TagResult)
				{
					productVM.Categories = await _context.Categories.ToListAsync();
					productVM.Tags = await _context.Tags.ToListAsync();
					productVM.Colors = await _context.Colors.ToListAsync();
					ModelState.AddModelError("TagIds", "There is no such tag");
					return View(productVM);
				}
			}
			foreach (int id in productVM.ColorIds)
			{
				bool colorResult = await _context.Colors.AnyAsync(t => t.Id == id);
				if (!colorResult)
				{
					productVM.Categories = await _context.Categories.ToListAsync();
					productVM.Tags = await _context.Tags.ToListAsync();
					productVM.Colors = await _context.Colors.ToListAsync();
					ModelState.AddModelError("ColorIds", "There is no such color");
					return View(productVM);
				}
			}
			foreach (int id in productVM.SizeIds)
			{
				bool sizeResult = await _context.Sizes.AnyAsync(t => t.Id == id);
				if (!sizeResult)
				{
					productVM.Categories = await _context.Categories.ToListAsync();
					productVM.Tags = await _context.Tags.ToListAsync();
					productVM.Colors = await _context.Colors.ToListAsync();
					ModelState.AddModelError("SizeIds", "There is no such size");
					return View(productVM);
				}
			}

			if (!productVM.MainPhoto.ValidateType())
			{
				productVM.Categories = await _context.Categories.ToListAsync();
				productVM.Tags = await _context.Tags.ToListAsync();
				productVM.Colors = await _context.Colors.ToListAsync();
				productVM.Sizes = await _context.Sizes.ToListAsync();
				ModelState.AddModelError("MainPhoto", "Wrong file type");
				return View(productVM);
			}
			if (!productVM.MainPhoto.ValidateSize(600))
			{
				productVM.Categories = await _context.Categories.ToListAsync();
				productVM.Tags = await _context.Tags.ToListAsync();
				productVM.Colors = await _context.Colors.ToListAsync();
				productVM.Sizes = await _context.Sizes.ToListAsync();
				ModelState.AddModelError("MainPhoto", "Wrong file size");
				return View(productVM);
			}
			if (!productVM.HoverPhoto.ValidateType())
			{
				productVM.Categories = await _context.Categories.ToListAsync();
				productVM.Tags = await _context.Tags.ToListAsync();
				productVM.Colors = await _context.Colors.ToListAsync();
				productVM.Sizes = await _context.Sizes.ToListAsync();
				ModelState.AddModelError("HoverPhoto", "Wrong file type");
				return View(productVM);
			}
			if (!productVM.HoverPhoto.ValidateSize(600))
			{
				productVM.Categories = await _context.Categories.ToListAsync();
				productVM.Tags = await _context.Tags.ToListAsync();
				productVM.Colors = await _context.Colors.ToListAsync();
				productVM.Sizes = await _context.Sizes.ToListAsync();
				ModelState.AddModelError("HoverPhoto", "Wrong file size");
				return View(productVM);
			}
			ProductImage image = new ProductImage
			{
                Alternative = productVM.Name,
                IsPrimary = true,
				Url = await productVM.MainPhoto.CreateFile(_env.WebRootPath, "assets", "images", "website-images")
			};
			ProductImage hoverImage = new ProductImage
			{
                Alternative = productVM.Name,
                IsPrimary = false,
				Url = await productVM.MainPhoto.CreateFile(_env.WebRootPath, "assets", "images", "website-images")
			};


			Product product = new Product
			{
				Name = productVM.Name,
				Price = productVM.Price,
				SKU = productVM.SKU,
				CategoryId = (int)productVM.CategoryId,
				Description = productVM.Description,
				ProductTags = new(),
				ProductColors = new(),
				ProductSizes = new(),
				ProductImages = new()
				{
					image,hoverImage
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
			foreach (int id in productVM.ColorIds)
			{
				var pColor = new ProductColor
				{
					ColorId = id,
					Product = product
				};
				product.ProductColors.Add(pColor);
			}
			foreach (int id in productVM.SizeIds)
			{
				var pSize = new ProductSize
				{
					SizeId = id,
					Product = product
				};
				product.ProductSizes.Add(pSize);
			}

			TempData["Message"] = "";
			foreach (IFormFile photo in productVM.Photos)
			{
				if (!photo.ValidateType())
				{
					TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file type wrong</p>";
					continue;
				}
				if (!photo.ValidateSize(600))
				{
					TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file size wrong</p>";
					continue;
				}

				product.ProductImages.Add(new ProductImage
				{
					Alternative = product.Name,
					IsPrimary = null,
					Url = await photo.CreateFile(_env.WebRootPath, "assets", "images", "website-images")
				});
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
			var existed = await _context.Products.Include(p=>p.ProductImages).FirstOrDefaultAsync(c => c.Id == id);
			if (existed is null) return NotFound();
			foreach (ProductImage image in existed.ProductImages)
			{
				image.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
			}
			_context.Products.Remove(existed);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Update(int id)
		{
			if(id<=0) return BadRequest();
			var product = await _context.Products
				.Include(p=>p.ProductTags)
				.Include(p=>p.ProductColors)
				.Include(p=>p.ProductSizes)
				.Include(p=>p.ProductImages)
				.FirstOrDefaultAsync(p => p.Id == id);
			if (product == null) return NotFound();
			var vm = new UpdateProductVM
			{
				Name = product.Name,
				Description = product.Description,
				Price = product.Price,
				SKU = product.SKU,
				CategoryId = product.CategoryId,
				ProductImages = product.ProductImages,
				TagIds = product.ProductTags.Select(pt=>pt.TagId).ToList(),
				ColorIds = product.ProductColors.Select(pc=>pc.ColorId).ToList(),
				SizeIds = product.ProductSizes.Select(ps=>ps.SizeId).ToList(),
				Categories = await _context.Categories.ToListAsync(),
				Tags= await _context.Tags.ToListAsync(),
				Colors= await _context.Colors.ToListAsync(),
				Sizes = await _context.Sizes.ToListAsync(),
			};
			return View(vm);
		}
		[HttpPost]
		public async Task<IActionResult> Update(int id,UpdateProductVM productVM)
		{
			Product existed = await _context.Products
				.Include(p => p.ProductTags)
				.Include(p => p.ProductColors)
				.Include(p => p.ProductSizes)
				.Include(p=>p.ProductImages)
				.FirstOrDefaultAsync(c => c.Id == id);
			productVM.ProductImages = existed.ProductImages;
			if (!ModelState.IsValid)
			{
				productVM.Categories= await _context.Categories.ToListAsync();
				productVM.Tags = await _context.Tags.ToListAsync();
				productVM.Colors = await _context.Colors.ToListAsync();
				productVM.Sizes = await _context.Sizes.ToListAsync();
				return View(productVM);
			}

			
			if (existed is null) return NotFound();

			bool result =await _context.Products.AnyAsync(c => c.CategoryId==productVM.CategoryId);
			if (!result)
			{
				productVM.Categories = await _context.Categories.ToListAsync();
				productVM.Tags = await _context.Tags.ToListAsync();
				productVM.Colors = await _context.Colors.ToListAsync();
				productVM.Sizes = await _context.Sizes.ToListAsync();
				return View(productVM);
			}

			foreach (int idT in productVM.TagIds)
			{
				bool TagResult = await _context.Tags.AnyAsync(t => t.Id == idT);
				if (!TagResult)
				{
					productVM.Categories = await _context.Categories.ToListAsync();
					productVM.Tags = await _context.Tags.ToListAsync();
					productVM.Colors = await _context.Colors.ToListAsync();
					productVM.Sizes = await _context.Sizes.ToListAsync();
					ModelState.AddModelError("TagIds", "There is no such tag");
					return View(productVM);
				}
			}
			foreach (int idC in productVM.ColorIds)
			{
				bool colorResult = await _context.Colors.AnyAsync(t => t.Id == idC);
				if (!colorResult)
				{
					productVM.Categories = await _context.Categories.ToListAsync();
					productVM.Tags = await _context.Tags.ToListAsync();
					productVM.Colors = await _context.Colors.ToListAsync();
					productVM.Sizes = await _context.Sizes.ToListAsync();
					ModelState.AddModelError("ColorIds", "There is no such color");
					return View(productVM);
				}
			}
			foreach (int idS in productVM.SizeIds)
			{
				bool sizeResult = await _context.Sizes.AnyAsync(t => t.Id == idS);
				if (!sizeResult)
				{
					productVM.Categories = await _context.Categories.ToListAsync();
					productVM.Tags = await _context.Tags.ToListAsync();
					productVM.Colors = await _context.Colors.ToListAsync();
					productVM.Sizes = await _context.Sizes.ToListAsync();
					ModelState.AddModelError("SizeIds", "There is no such size");
					return View(productVM);
				}
			}


			result = _context.Products.Any(c => c.Name == productVM.Name && c.Id != id);
			if (result)
			{
				productVM.Categories = await _context.Categories.ToListAsync();
				productVM.Tags = await _context.Tags.ToListAsync();
				productVM.Colors = await _context.Colors.ToListAsync();
				productVM.Sizes = await _context.Sizes.ToListAsync();
				ModelState.AddModelError("Name", "There is already such product");
				return View(productVM);
			}

			if(productVM.MainPhoto is not null)
			{
				if(!productVM.MainPhoto.ValidateType())
				{
					productVM.Categories = await _context.Categories.ToListAsync();
					productVM.Tags = await _context.Tags.ToListAsync();
					productVM.Colors = await _context.Colors.ToListAsync();
					productVM.Sizes = await _context.Sizes.ToListAsync();
					ModelState.AddModelError("MainPhoto", "File type is not valid");
					return View(productVM);
				}
				if (!productVM.MainPhoto.ValidateSize(600))
				{
					productVM.Categories = await _context.Categories.ToListAsync();
					productVM.Tags = await _context.Tags.ToListAsync();
					productVM.Colors = await _context.Colors.ToListAsync();
					productVM.Sizes = await _context.Sizes.ToListAsync();
					ModelState.AddModelError("MainPhoto", "File size is not valid");
					return View(productVM);
				}
			}
			if (productVM.HoverPhoto is not null)
			{
				if (!productVM.HoverPhoto.ValidateType())
				{
					productVM.Categories = await _context.Categories.ToListAsync();
					productVM.Tags = await _context.Tags.ToListAsync();
					productVM.Colors = await _context.Colors.ToListAsync();
					productVM.Sizes = await _context.Sizes.ToListAsync();
					ModelState.AddModelError("HoverPhoto", "File type is not valid");
					return View(productVM);
				}
				if (!productVM.HoverPhoto.ValidateSize(600))
				{
					productVM.Categories = await _context.Categories.ToListAsync();
					productVM.Tags = await _context.Tags.ToListAsync();
					productVM.Colors = await _context.Colors.ToListAsync();
					productVM.Sizes = await _context.Sizes.ToListAsync();
					ModelState.AddModelError("HoverPhoto", "File size is not valid");
					return View(productVM);
				}
			}

			if(productVM.MainPhoto is not null)
			{
				string fileName = await productVM.MainPhoto.CreateFile(_env.WebRootPath, "assets", "images", "website-images");
				ProductImage mainImage = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true);
				mainImage.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
				_context.ProductImages.Remove(mainImage);
				existed.ProductImages.Add(new ProductImage
				{
					Alternative = productVM.Name,
					IsPrimary = true,
					Url = fileName
				});
			}
			if (productVM.HoverPhoto is not null)
			{
				string fileName = await productVM.HoverPhoto.CreateFile(_env.WebRootPath, "assets", "images", "website-images");
				ProductImage hoverImage = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true);
				hoverImage.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
				_context.ProductImages.Remove(hoverImage);
				existed.ProductImages.Add(new ProductImage
				{
					Alternative = productVM.Name,
					IsPrimary = false,
					Url = fileName
				});
			}
			if(productVM.ImageIds is null)
			{
				productVM.ImageIds = new();
			}
			var removeable = existed.ProductImages.Where(pi => !productVM.ImageIds.Exists(imgId => imgId == pi.Id ) && pi.IsPrimary == null).ToList();
			foreach (ProductImage pi in removeable)
			{
				pi.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
				existed.ProductImages.Remove(pi);
			}

			existed.ProductTags.RemoveAll(pTag => !productVM.TagIds.Contains(pTag.Id));
			
			existed.ProductTags.AddRange(productVM.TagIds.Where(tagId => !existed.ProductTags.Any(pt => pt.Id == tagId))
								 .Select(tagId => new ProductTag { TagId = tagId }));

			existed.ProductColors.RemoveAll(pColor => !productVM.ColorIds.Contains(pColor.Id));

			existed.ProductColors.AddRange(productVM.ColorIds.Where(colorId => !existed.ProductColors.Any(pt => pt.Id == colorId))
								 .Select(colorId => new ProductColor { ColorId = colorId }));

			existed.ProductSizes.RemoveAll(pSize => !productVM.SizeIds.Contains(pSize.Id));

			existed.ProductSizes.AddRange(productVM.SizeIds.Where(sizeId => !existed.ProductSizes.Any(pt => pt.Id == sizeId))
								 .Select(sizeId => new ProductSize { SizeId = sizeId }));

			TempData["Message"] = "";
			if (productVM.Photos is not null)
			{
				foreach (IFormFile photo in productVM.Photos)
				{
					if (!photo.ValidateType())
					{
						TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file type wrong</p>";
						continue;
					}
					if (!photo.ValidateSize(600))
					{
						TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file size wrong</p>";
						continue;
					}

					existed.ProductImages.Add(new ProductImage
					{
						Alternative = productVM.Name,
						IsPrimary = null,
						Url = await photo.CreateFile(_env.WebRootPath, "assets", "images", "website-images")
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
