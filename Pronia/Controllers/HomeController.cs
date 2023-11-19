using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Models;
using Pronia.ViewModels;

namespace Pronia.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Slide> Slides = _context.Slides.OrderBy(s => s.Order).ToList();
            List<Product> Products = _context.Products.Include(p => p.ProductImages).ToList();
            Console.WriteLine(Products[0].ProductImages.Count);

            HomeViewModel vm = new()
            {
                Slides = Slides,
                Products= Products
            };
            return View(vm);
        }

        public IActionResult About()
        {
            return View();
        }
    }
}
