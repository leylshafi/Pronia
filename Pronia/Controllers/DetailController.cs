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

        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0) return BadRequest();


            Product product = await _context.Products
                .Include(p=>p.ProductColors).ThenInclude(pc=>pc.Color)
                .Include(p=>p.ProductSizes).ThenInclude(ps=>ps.Size)
                .Include(p=>p.ProductImages)
                .Include(p=>p.ProductTags).ThenInclude(pt=>pt.Tag)
                .Include(p=>p.Category).FirstOrDefaultAsync(p => p.Id == id);
            List<Product> RelatedProducts = await _context.Products.Include(p=>p.ProductImages).Where(p => p.CategoryId == product.CategoryId && p.Id!=product.Id).ToListAsync();
            Console.WriteLine(RelatedProducts.Count);

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
