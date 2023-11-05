using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Data;
using Shopping.Data.Entities;

namespace Shopping.Controllers
{
    public class OrdersController : Controller
    {
        private readonly DataContext _context;

        public OrdersController(DataContext context)
        {
          _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sales.Include(x=>x.User).Include(x=>x.SaleDetails)
                .ThenInclude(x=>x.Product).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }
            Sale sale = await _context.Sales.Include(x => x.User)
                .Include(x => x.SaleDetails).ThenInclude(x => x.Product)
                .ThenInclude(x => x.ProductImages).FirstOrDefaultAsync(x => x.Id == id);
            if(sale == null) { return NotFound(); }

            return View(sale);

        }
    }
}
