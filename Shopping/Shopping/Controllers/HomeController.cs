﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
                if (cart!=null)
            {
                ViewBag.cart = cart;
                //ViewBag.total = cart.Sum(item => (int)(item.Product.Price) * item.Quantity);
            }
            float quantity = 0;
           
            if (cart != null)
            {
              

                quantity = dataCart.Sum(ts => ts.Quantity);
            }
           
            HomeViewModel model = new() { Products = products, Quantity=quantity};
            

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
                    Quantity = 1,
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
                        dataCart[i].Quantity++;
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
            var cart = SessionHelper.GetObjectFromJson<List<TemporalSale>>(HttpContext.Session, "cart");
            if (cart != null)
            {
                ViewBag.cart = cart;
                ViewBag.total = cart.Sum(item => (int)(item.Product.Price) * item.Quantity);
            }
            float quantity = 0;

            if (cart != null)
            {
                quantity = cart.Sum(ts => ts.Quantity);
            }
            ShowCartViewModel model = new()
            {
              
                TemporalSales = cart,
            };

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
            List<TemporalSale> cart = SessionHelper.GetObjectFromJson<List<TemporalSale>>(HttpContext.Session, "cart");

            if (cart != null && id.HasValue)
            {
                for (int i = 0; i < cart.Count; i++)
                {
                    if (cart[i].Product != null && cart[i].Product.Id == id)
                    {
                        return i; // Return the index of the existing item
                    }
                }
            }

            return -1; // Return -1 if the item doesn't exist in the cart or if the cart is empty
        }
        
    }
}