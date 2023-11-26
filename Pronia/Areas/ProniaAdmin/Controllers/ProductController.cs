﻿using Microsoft.AspNetCore.Mvc;
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

			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Create(CreateProductVM productVM)
		{
			if (!ModelState.IsValid)
			{
                ViewBag.Categories = await _context.Categories.ToListAsync();
                return View();
			}
			bool result = await _context.Categories.AnyAsync(c => c.Id == productVM.CategoryId);
			if(!result) 
			{
                ViewBag.Categories = await _context.Categories.ToListAsync();
                ModelState.AddModelError("CategoryId", "There is no such category");
				return View();
			}

			Product product = new Product
			{
				Name = productVM.Name,
				Price = productVM.Price,
				SKU = productVM.SKU,
				CategoryId = (int)productVM.CategoryId,
				Description = productVM.Description
			};

			await _context.Products.AddAsync(product);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
	}
}