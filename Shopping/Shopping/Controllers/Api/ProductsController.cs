using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Data;
using Shopping.Data.Entities;
using Shopping.Models.Response;

namespace Shopping.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly DataContext _context;

        public ProductsController(DataContext context)
        {
           _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<Product> product = await _context.Products.Include(x => x.ProductImages)
                 .Include(x => x.ProductCategories).ThenInclude(x => x.Category)
                .ToListAsync();
            return Ok(product);
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> getProductbyCategory(int id)
        {
            var products = await _context.Categories.Include(x => x.ProductCategories)
                .ThenInclude(x => x.Product).FirstOrDefaultAsync(x => x.Id == id);


            var response = new CategoryResponse
            {
                Id= products.Id,
                Name = products.Name,
                Products = products.ProductCategories?.Select(pc => new ProductResponse
                {
                    Id=pc.Product.Id,
                    Description=pc.Product.Description,
                    Name=pc.Product.Name,
                    Price=pc.Product.Price,
                    Stock=pc.Product.Stock,
                    ProductImages=pc.Product.ProductImages?.Select(x=> new ProductImagesResponse
                    {
                        Id=x.Id,
                        ImageId=x.ImageId,                       

                    }).ToList(),

                }).ToList(),
            };
           
            return Ok(response);
        }

    }
}
