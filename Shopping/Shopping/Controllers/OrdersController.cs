
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Data;
using Shopping.Data.Entities;
using Shopping.Enum;
using Shopping.Helpers;
using Vereyon.Web;

namespace Shopping.Controllers
{
    public class OrdersController : Controller
    {
        private readonly DataContext _context;
        private readonly IFlashMessage _flashMessage;
        private readonly IOrdersHelper _ordersHelper;

        public OrdersController(DataContext context, IFlashMessage flashMessage, IOrdersHelper ordersHelper)

        {
          _context = context;
           _flashMessage = flashMessage;
            _ordersHelper = ordersHelper;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sales.Include(x=>x.User).Include(x=>x.SaleDetails)
                .ThenInclude(x=>x.Product).ToListAsync());
        }
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Dispatch(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Sale sale = await _context.Sales.FindAsync(id);
            if (sale == null) { return NotFound(); }
            if (sale.OrderStatus != OrderStatus.New)
            {
                _flashMessage.Danger("Only orders that are in 'new' status can be shipped.");
            }
            else
            {
                sale.OrderStatus = OrderStatus.Delivered;
                _context.Sales.Update(sale);
                await _context.SaveChangesAsync();
                _flashMessage.Confirmation("The order status has been changed to 'shipped'.");

            }
            return RedirectToAction(nameof(Details), new {id=sale.Id});
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Send(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Sale sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }

            if (sale.OrderStatus != OrderStatus.Delivered)
            {
                _flashMessage.Danger("Only orders that are in 'dispatched' status can be shipped.");
            }
            else
            {
                sale.OrderStatus = OrderStatus.Sent;
                _context.Sales.Update(sale);
                await _context.SaveChangesAsync();
                _flashMessage.Confirmation("The order status has been changed to 'shipped'.");
            }

            return RedirectToAction(nameof(Details), new { Id = sale.Id });
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Confirm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Sale sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }

            if (sale.OrderStatus != OrderStatus.Sent)
            {
                _flashMessage.Danger("Only orders that are in 'shipped' status can be confirmed.\"");
            }
            else
            {
                sale.OrderStatus = OrderStatus.Confirmed;
                _context.Sales.Update(sale);
                await _context.SaveChangesAsync();
                _flashMessage.Confirmation("The order status has been changed to 'confirmed'.");
            }

            return RedirectToAction(nameof(Details), new { Id = sale.Id });
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Sale sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }

            if (sale.OrderStatus == OrderStatus.Cancelled)
            {
                _flashMessage.Danger("You cannot cancel an order that is in 'cancelled' status.");
            }
            else
            {
                await _ordersHelper.CancelOrderAsync(sale.Id);
                _flashMessage.Confirmation("The order status has been changed to 'cancelled'.");
            }

            return RedirectToAction(nameof(Details), new { Id = sale.Id });
        }
        [Authorize(Roles = "User")]
        public async Task<IActionResult> MyOrders()
        {
            return View(await _context.Sales.Include(x=>x.User)
                .Include(x=>x.SaleDetails).ThenInclude(x=>x.Product)
                .Where(x=>x.User.UserName==User.Identity.Name).ToListAsync());
        }

        [Authorize(Roles ="User")]
        public async Task<IActionResult> MyDetails(int? Id)
        {
            if(Id==null)
            {
                return NotFound();
            }
            Sale sale = await _context.Sales.Include(x => x.User)
                .Include(x => x.SaleDetails).ThenInclude(x => x.Product)
                .ThenInclude(x => x.ProductImages).FirstOrDefaultAsync(x => x.Id == Id);
           if(sale==null) { return NotFound(); }
           return View(sale);
        }
    }
}
