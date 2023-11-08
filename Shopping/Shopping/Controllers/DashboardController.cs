using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shopping.Data;
using Shopping.Data.Entities;
using Shopping.Enum;
using Shopping.Helpers;

namespace Shopping.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public DashboardController(DataContext context , IUserHelper userHelper)
        {
           _context = context;
           _userHelper = userHelper;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.UsersCount = _context.Users.Count();
            ViewBag.ProductsCount = _context.Products.Count();
            ViewBag.NewOrdersCount = _context.Sales.Where(o => o.OrderStatus == OrderStatus.New).Count();
            ViewBag.ConfirmedOrdersCount = _context.Sales.Where(o => o.OrderStatus == OrderStatus.Confirmed).Count();
            var cart = HttpContext.Session.GetString("cart");//get key cart
            if (cart == null)
            {
                TemporalSale model1 = new()
                {
                    
                };

                return View(model1);
            }
            List<TemporalSale> dataCart = JsonConvert.DeserializeObject<List<TemporalSale>>(cart);
            if (cart != null)
            {
                float quantity = 0;

                if (cart != null)
                {
                    quantity = dataCart.Sum(ts => ts.Quantity);
                }


              List<TemporalSale> model1 = new()
                {

                };
                model1 = dataCart;
                return View(model1);
            }


                
                return View();

        }
    }
}
