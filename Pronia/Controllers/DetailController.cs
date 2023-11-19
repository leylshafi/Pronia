using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Models;
using Pronia.ViewModels;

namespace Pronia.Controllers
{
    public class DetailController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}
        private readonly AppDbContext _context;

        public DetailController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Detail(int id)
        {
            if (id <= 0) return BadRequest();


            Product product = _context.Products.Include(p=>p.Category).FirstOrDefault(p => p.Id == id);
            List<Product> RelatedProducts = _context.Products.Include(p=>p.ProductImages).Where(p => p.CategoryId == product.CategoryId).ToList();

            DetailVM vm = new DetailVM()
            {
                Product = product,
                RelatedProducts = RelatedProducts
            };
            if (product == null) return NotFound();



            return View(vm);
        }
    }
}
