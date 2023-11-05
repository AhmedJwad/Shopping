using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Newtonsoft.Json;
using Shopping.Common;
using Shopping.Data;
using Shopping.Data.Entities;
using Shopping.Enum;
using Shopping.Models;

namespace Shopping.Helpers
{
    public class OrdersHelper : IOrdersHelper
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public OrdersHelper(DataContext context, IUserHelper userHelper)
        {
           _context = context;
           _userHelper = userHelper;
        }

        public async Task<Response> CancelOrderAsync(int id)
        {
           Sale sale=await _context.Sales.Include(x=>x.SaleDetails)
                .ThenInclude(x=>x.Product).FirstOrDefaultAsync(x=>x.Id==id);
            foreach (SaleDetail item in sale.SaleDetails)
            {
                Product product = await _context.Products.FindAsync(item.Product.Id);
                if (product != null)
                {
                    product.Stock += item.Quantity;
                }
            }
            sale.OrderStatus = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();
            return new Response { IsSuccess = true };
        }

        public async Task<Response> ProcessOrderAsync(ShowCartViewModel model)
        {
            Response response = await CheckInventoryAsync(model);
            var user = await _userHelper.GetUserAsync(model.username);            

            if (!response.IsSuccess) { return response; }
           int saleDetailIdCounter = 0; // Initialize the counter with the starting ID
            Sale sale = new Sale()
            {
                Date = DateTime.UtcNow,               
                Remarks = model.Remarks,
                SaleDetails = new List<SaleDetail>(),
                OrderStatus = OrderStatus.New,   
                 User = user,
               
            };           

           
            foreach (var item in model.TemporalSales)
            {
                if (saleDetailIdCounter == 0)
                {
                    // If there are existing SaleDetails, find the last ID and increment it
                    saleDetailIdCounter = 1;
                }
                else
                {
                    saleDetailIdCounter = sale.SaleDetails.Max(sd => sd.Id) + 1;
                }
                var saleDetail = new SaleDetail
                {
                   
                    Product =await _context.Products.FindAsync(item.Product.Id), // This assumes that the Product property is a navigation property
                    Quantity = item.Quantity,
                    Remarks = item.Remarks,
                };

                sale.SaleDetails.Add(saleDetail);

                Product product =await  _context.Products.FindAsync(item.Product.Id);             
                               
                if (product != null)
                {
                    product.Stock -= item.Quantity;
                    _context.Products.Update(product);

                    
                }
               
            }

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();
            return response;

        }

        private async Task<Response> CheckInventoryAsync(ShowCartViewModel model)
        {
            Response response = new() { IsSuccess = true };
            foreach (var item in model.TemporalSales)
            {
                Product product = await _context.Products.FindAsync(item.Product.Id);
                if (product == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"The product {product.Name} is no longer available";
                    return response;
                }
                if(product.Stock< item.Quantity)
                {
                    response.IsSuccess = false;
                    response.Message = $"We are sorry that we do not have enough stock of the product" +
                        $" {item.Product.Name} " +
                        $"to take your order. Please reduce the quantity or replace it with another";
                    return response;
                }
               
            }
            return response;
        }
    }
}
