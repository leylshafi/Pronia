using Microsoft.AspNetCore.Mvc;
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
            //List<Slide> Slides = new List<Slide>() {
            //    new Slide()
            //    {
            //        Subtitle = "65% OFF",
            //        Title = "NEW PLANT",
            //        Description = "Pronia, With 100% Natural, Organic & Plant Shop.",
            //        ImageUrl = "/assets/images/website-images//1-2-524x617.png",
            //        Order=1
            //    },
            //    new Slide()
            //    {
            //        Subtitle = "65% OFF",
            //        Title = "NEW PLANT",
            //        Description = "Pronia, With 100% Natural, Organic & Plant Shop.",
            //        ImageUrl = "/assets/images/website-images//1-1-524x617.png",
            //        Order=2
            //    },


            //};
            //_context.Slides.AddRange(Slides);
            //_context.SaveChanges();

            List<Product> Products = new()
            {
                new Product()
                {
                    CreatedTime = DateTime.Now.AddMinutes(5),
                    Name = "American Marigold",
                    Price = 23.45m,
                    PrimaryImageUrl="/assets/images/website-images//1-1-270x300.jpg",
                    SecondaryImageUrl = "/assets/images/website-images//1-2-270x300.jpg"

                },
                new Product()
                {
                    CreatedTime = DateTime.Now.AddMinutes(2),
                    Name = "Black Eyed Susan",
                    Price = 25.45m,
                    PrimaryImageUrl="/assets/images/website-images//1-2-270x300.jpg",
                    SecondaryImageUrl = "/assets/images/website-images//1-3-270x300.jpg"

                },
                new Product()
                {
                    CreatedTime = DateTime.Now.AddMinutes(1),
                    Name = "Bleeding Heart",
                    Price = 30.45m,
                    PrimaryImageUrl="/assets/images/website-images//1-3-270x300.jpg",
                    SecondaryImageUrl = "/assets/images/website-images//1-4-270x300.jpg"

                },
                new Product()
                {
                    CreatedTime = DateTime.Now.AddMinutes(10),
                    Name = "Bloody Cranesbill",
                    Price = 45.00m,
                    PrimaryImageUrl="/assets/images/website-images//1-4-270x300.jpg",
                    SecondaryImageUrl = "/assets/images/website-images//1-5-270x300.jpg"

                },
                new Product()
                {
                    CreatedTime = DateTime.Now.AddMinutes(6),
                    Name = "Butterfly Weed",
                    Price = 50.45m,
                    PrimaryImageUrl="/assets/images/website-images//1-5-270x300.jpg",
                    SecondaryImageUrl = "/assets/images/website-images//1-6-270x300.jpg"

                },
                new Product()
                {
                    CreatedTime = DateTime.Now.AddMinutes(8),
                    Name = "Common Yarrow",
                    Price = 65.00m,
                    PrimaryImageUrl="/assets/images/website-images//1-6-270x300.jpg",
                    SecondaryImageUrl = "/assets/images/website-images//1-7-270x300.jpg"

                },
                new Product()
                {
                    CreatedTime = DateTime.Now.AddMinutes(13),
                    Name = "Doublefile Viburnum",
                    Price = 67.45m,
                    PrimaryImageUrl="/assets/images/website-images//1-7-270x300.jpg",
                    SecondaryImageUrl = "/assets/images/website-images//1-8-270x300.jpg"

                },
                 new Product()
                {
                    CreatedTime = DateTime.Now.AddMinutes(11),
                    Name = "Feather Reed Grass",
                    Price = 20.00m,
                    PrimaryImageUrl="/assets/images/website-images//1-8-270x300.jpg",
                    SecondaryImageUrl = "/assets/images/website-images//1-1-270x300.jpg"

                }

            };
            _context.Products.AddRange(Products);
            _context.SaveChanges();


            HomeViewModel vm = new()
            {
                Slides = _context.Slides.OrderBy(s => s.Order).ToList(),
                Products=_context.Products.ToList()
            };
            return View(vm);
        }

        public IActionResult About()
        {
            return View();
        }
    }
}
