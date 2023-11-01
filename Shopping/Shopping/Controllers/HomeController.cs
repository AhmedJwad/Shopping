using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shopping.Common;
using Shopping.Data;
using Shopping.Data.Entities;
using Shopping.Helpers;
using Shopping.Models;
using System.Diagnostics;
using Response = Shopping.Common.Response;

namespace Shopping.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IOrdersHelper _ordersHelper;

        public string ShoppingCartId { get; set; }
        public const string CartSessionKey = "CartId";

        public HomeController(ILogger<HomeController> logger, DataContext context , IUserHelper userHelper ,
            IOrdersHelper ordersHelper )
        {
            _logger = logger;
            _context = context;
           _userHelper = userHelper;
            _ordersHelper = ordersHelper;
        }

        public async Task<IActionResult> Index()
        {
            List<Product> products = await _context.Products
                .Include(x=>x.ProductCategories).Include(x=>x.ProductImages).Where(p => p.Stock > 0)
                .OrderBy(x=>x.Description).ToListAsync();


            var cart = HttpContext.Session.GetString("cart");//get key cart
            if (cart == null)
            {
                HomeViewModel model1 = new() { Products = products };


                return View(model1);
            }
            List<TemporalSale> dataCart = JsonConvert.DeserializeObject<List<TemporalSale>>(cart);
            if (cart != null)
            {

                if (dataCart.Count > 0)
                {
                    ViewBag.carts = dataCart;

                }
            }
            if (cart != null)
            {
                ViewBag.cart = cart;
                //ViewBag.total = cart.Sum(item => (int)(item.Product.Price) * item.Quantity);
            }
            float quantity = 0;

            if (cart != null)
            {


                quantity = dataCart.Sum(ts => ts.Quantity);
            }


            HomeViewModel model = new() { Products = products,Quantity=quantity};
           


            return View(model);

        }
        public async Task<IActionResult> Details(int? Id)
        {
            if(Id==null)
            {
                return NotFound();
            }

            Product product = await _context.Products.Include(x => x.ProductImages)
                .Include(x => x.ProductCategories).ThenInclude(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == Id);
            if (product == null) { return NotFound(); }
            string categories = string.Empty;

            foreach (ProductCategory? category  in product.ProductCategories)
            {
                categories += $"{category.Category.Name}";

            }
            categories = categories.Substring(0, categories.Length - 2);
            AddProductToCartViewModel model = new()
            {
                Categories = categories,
                Description = product.Description,
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                ProductImages = product.ProductImages,
                Quantity = 1,
                Stock = product.Stock,
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(AddProductToCartViewModel model)
        {
            

            Product product = await _context.Products.FindAsync(model.Id);
            if (product == null)
            {
                return NotFound();
            }
            var cart = HttpContext.Session.GetString("cart");//get key cart
            if (cart == null)
            {
                List<TemporalSale> Ahmed = new List<TemporalSale>()
                {
                    new TemporalSale
                    {
                    
                    Product = product,
                    Quantity = model.Quantity == 0 ? 1 : model.Quantity,
                    }
                    
                };
                

                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(Ahmed));
            }
            else
            {
                List<TemporalSale> dataCart = JsonConvert.DeserializeObject<List<TemporalSale>>(cart);
               
                bool check = true;
                for (int i = 0; i < dataCart.Count; i++)
                {
                    if (dataCart[i].Product.Id == model.Id)
                    {
                        dataCart[i].Quantity=model.Quantity;
                        check = false;
                    }
                }
                if (check)
                {
                    dataCart.Add(new TemporalSale
                    {
                        Product = product,
                        Quantity = 1
                    });
                }

              

                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));

            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ShowCart()
        {
           
            var cart = HttpContext.Session.GetString("cart");
            if(cart !=null)
            {
                List<TemporalSale> dataCart = JsonConvert.DeserializeObject<List<TemporalSale>>(cart);

                ShowCartViewModel model = new()
                {

                    TemporalSales = dataCart,
                };
                return View(model);
            }
            else
            {
                List<TemporalSale> list = new List<TemporalSale>();
                ShowCartViewModel model = new()
                {
                   
                    TemporalSales= list,
                };
                return View(model);
            }      
                     
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShowCart(ShowCartViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            User user = await _userHelper.GetUserAsync(User.Identity.Name);
            if (user == null) { return NotFound(); }


            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                List<TemporalSale> dataCart = JsonConvert.DeserializeObject<List<TemporalSale>>(cart);


                model.TemporalSales = dataCart;
                model.username = user.Email;


                Response response = await _ordersHelper.ProcessOrderAsync(model);

                if (response.IsSuccess)
                {
                    return RedirectToAction(nameof(OrderSuccess));
                }

                ModelState.AddModelError(string.Empty, response.Message);
            }


            return View(model);



        }

        public async Task<IActionResult> DecreaseQuantity(int? Id)
        {
            if(Id== null)
            {
                return NotFound();
            }

          
            var cart = HttpContext.Session.GetString("cart");
            if(cart !=null)
            {
                List<TemporalSale> datacart = JsonConvert.DeserializeObject<List<TemporalSale>>(cart);
               var temporalSales = await _context.TemporalSales.ToListAsync();


                if (datacart.Count > 0)
                {

                    foreach (var item2 in datacart)
                    {

                        if (item2.Product.Id == Id)
                        {
                            if(item2.Quantity>0)
                            {
                                item2.Quantity--;
                            }                           
                        }
                        
                    }


                }

                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(datacart));
                var cart2 = HttpContext.Session.GetString("cart");
            }
            return RedirectToAction(nameof(ShowCart));

        }
        public async Task<IActionResult> IncreaseQuantity(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                List<TemporalSale> datacart = JsonConvert.DeserializeObject<List<TemporalSale>>(cart);

                foreach (TemporalSale item in datacart)
                {
                  
                        if (item.Product.Id == Id)
                        {
                            item.Quantity++;
                        }
                   

                }
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(datacart));

                var cart2 = HttpContext.Session.GetString("cart");
            }
            return RedirectToAction(nameof(ShowCart));

        }
        public async Task<IActionResult>Delete(int? Id)
        {
            if(Id==null)
            {
                return NotFound();
            }

            var cart = HttpContext.Session.GetString("cart");
            if(cart != null)
            {
                List<TemporalSale> datacart = JsonConvert.DeserializeObject<List<TemporalSale>>(cart);
                for (int i = 0; i < datacart.Count; i++)
                {
                    if (datacart[i].Product.Id == Id)
                    {
                        datacart.RemoveAt(i);
                    }
                }
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(datacart));
            }
            return RedirectToAction(nameof(ShowCart));
        }

        [Authorize]
        public IActionResult OrderSuccess()
        {
            return View();
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

            var cart = HttpContext.Session.GetString("cart");//get key cart
            if (cart == null)
            {

                List<TemporalSale> Ahmed = new List<TemporalSale>()
                {
                    new TemporalSale
                    {
                    Id=product.Id + 20,
                    Product = product,
                    Quantity = 1,
                    }

                };

                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(Ahmed));
            }
            else
            {

                List<TemporalSale> dataCart = JsonConvert.DeserializeObject<List<TemporalSale>>(cart);
                int nextId = dataCart.Count + 1;
                bool check = true;
                for (int i = 0; i < dataCart.Count; i++)
                {
                    if (dataCart[i].Product.Id == product.Id)
                    {
                        dataCart[i].Id = nextId + 20;
                        dataCart[i].Quantity++;
                        check = false;
                    }

                }
                if (check)
                {
                    dataCart.Add(new TemporalSale
                    {
                        Id = nextId + 20,
                        Product = product,
                        Quantity = 1
                    });
                }

                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));

            }

            return RedirectToAction(nameof(Index));



        }






    }
}