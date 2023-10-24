using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Shopping.Data;
using Shopping.Data.Entities;
using Shopping.Helpers;
using Shopping.Migrations;
using Shopping.Models;
using System.Diagnostics;

namespace Shopping.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _context;
       

        public string ShoppingCartId { get; set; }
        public const string CartSessionKey = "CartId";

        public HomeController(ILogger<HomeController> logger, DataContext context )
        {
            _logger = logger;
            _context = context;
          
        }

        public async Task<IActionResult> Index()
        {
            List<Product> products = await _context.Products
                .Include(x=>x.ProductCategories).Include(x=>x.ProductImages)
                .OrderBy(x=>x.Description).ToListAsync();
            List<ProductsHomeViewModel> productsHomes = new() { new ProductsHomeViewModel() };
            int i = 1;
            foreach (var product in products)
            {
                if(i==1)
                {
                    productsHomes.LastOrDefault().Product1 = product;
                }
                if(i==2)
                {
                    productsHomes.LastOrDefault().Product2 = product;
                }
                if(i==3)
                {
                    productsHomes.LastOrDefault().Product3 = product;
                }
                if(i==4)
                {
                    productsHomes.LastOrDefault().Product4 = product;
                    productsHomes.Add(new ProductsHomeViewModel() );
                    i = 0;
                }
                i++;
            }
           
            
            
            var cart = SessionHelper.GetObjectFromJson<List<TemporalSale>>(HttpContext.Session, "cart");
            if(cart!=null)
            {
                ViewBag.cart = cart;
                ViewBag.total = cart.Sum(item => (int)(item.Product.Price) * item.Quantity);
            }
            float quantity = 0;
           
            if (cart != null)
            {
                quantity = cart.Sum(ts => ts.Quantity);
            }
           
            HomeViewModel model = new() { Products = productsHomes , Quantity=quantity};
            

            return View(model);

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

		[Route("error/404")]
		public IActionResult Error404()
		{
			return View();
		}
        public async Task<IActionResult> Add(int? id)
        {
            
            if (id == null)
            {
                return NotFound();
            }

            //if (!User.Identity.IsAuthenticated)
            //{
            //    return RedirectToAction("Login", "Account");
            //}

            Product product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            string userIdentifier = Guid.NewGuid().ToString();

          

            if (SessionHelper.GetObjectFromJson<List<TemporalSale>>(HttpContext.Session, "cart") == null)
            {
                List<TemporalSale> cart = new List<TemporalSale>();
                cart.Add(new TemporalSale { Product = product, Quantity = 1, });
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);



            }
            else
            {
                List<TemporalSale> cart = SessionHelper.GetObjectFromJson<List<TemporalSale>>(HttpContext.Session, "cart");

                int index = isExist(id);
                if (index != -1)
                {
                    cart[index].Quantity++;
                }



                else
                {
                    cart.Add(new TemporalSale { Product = product, Quantity = 1 });
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);

            }
           
            return RedirectToAction(nameof(Index));
        }

        

      
        private int isExist(int? id)
        {
            var Ahmed =  _context.TemporalSales.ToList();
            for (int i = 0; i < Ahmed.Count; i++)
            {
                if (Ahmed[i].Product.Id.Equals(id))
                {
                    return i;
                }
            }
            return -1;
        }
        
    }
}