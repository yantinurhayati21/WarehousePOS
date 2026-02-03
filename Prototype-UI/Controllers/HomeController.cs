using Microsoft.AspNetCore.Mvc;
using Prototype_UI.Models;
using System.Diagnostics;

namespace Prototype_UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Nasi Goreng Spesial", Category = "Makanan Utama", Price = 35000, Stock = 50, ImageUrl = "https://images.unsplash.com/photo-1512058564366-18510be2db19?w=200&h=200&fit=crop" },
                new Product { Id = 2, Name = "Mie Ayam Bakso", Category = "Makanan Utama", Price = 28000, Stock = 45, ImageUrl = "https://images.unsplash.com/photo-1569718212165-3a8278d5f624?w=200&h=200&fit=crop" },
                new Product { Id = 3, Name = "Sate Ayam Madura", Category = "Makanan Utama", Price = 40000, Stock = 30, ImageUrl = "https://images.unsplash.com/photo-1529563021893-cc83c992d75d?w=200&h=200&fit=crop" },
                new Product { Id = 4, Name = "Rendang Sapi", Category = "Makanan Utama", Price = 55000, Stock = 25, ImageUrl = "https://images.unsplash.com/photo-1606491956689-2ea866880c84?w=200&h=200&fit=crop" },
                new Product { Id = 5, Name = "Es Teh Manis", Category = "Minuman", Price = 8000, Stock = 100, ImageUrl = "https://images.unsplash.com/photo-1556679343-c7306c1976bc?w=200&h=200&fit=crop" },
                new Product { Id = 6, Name = "Es Jeruk Peras", Category = "Minuman", Price = 12000, Stock = 80, ImageUrl = "https://images.unsplash.com/photo-1621506289937-a8e4df240d0b?w=200&h=200&fit=crop" },
                new Product { Id = 7, Name = "Kopi Susu Gula Aren", Category = "Minuman", Price = 22000, Stock = 60, ImageUrl = "https://images.unsplash.com/photo-1461023058943-07fcbe16d735?w=200&h=200&fit=crop" },
                new Product { Id = 8, Name = "Jus Alpukat", Category = "Minuman", Price = 18000, Stock = 40, ImageUrl = "https://images.unsplash.com/photo-1524351199678-941a58a3df50?w=200&h=200&fit=crop" },
                new Product { Id = 9, Name = "Kentang Goreng", Category = "Snack", Price = 20000, Stock = 35, ImageUrl = "https://images.unsplash.com/photo-1573080496219-bb080dd4f877?w=200&h=200&fit=crop" },
                new Product { Id = 10, Name = "Roti Bakar Coklat", Category = "Snack", Price = 15000, Stock = 40, ImageUrl = "https://images.unsplash.com/photo-1585459512398-eb0d6aec47b9?w=200&h=200&fit=crop" },
                new Product { Id = 11, Name = "Es Krim Cone", Category = "Dessert", Price = 12000, Stock = 50, ImageUrl = "https://images.unsplash.com/photo-1497034825429-c343d7c6a68f?w=200&h=200&fit=crop" },
                new Product { Id = 12, Name = "Pisang Goreng Keju", Category = "Snack", Price = 18000, Stock = 45, ImageUrl = "https://images.unsplash.com/photo-1628949371682-1b396b54ed11?w=200&h=200&fit=crop" }
            };
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
