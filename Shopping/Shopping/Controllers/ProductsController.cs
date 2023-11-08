using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Shopping.Data;
using Shopping.Data.Entities;
using Shopping.Helpers;
using Shopping.Models;
using Vereyon.Web;
using static Shopping.Helpers.ModalHelper;

namespace Shopping.Controllers
{
    public class ProductsController : Controller
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IFlashMessage _flashMessage;

        public ProductsController(DataContext context, ICombosHelper combosHelper,
            IBlobHelper blobHelper, IFlashMessage flashMessage)
        {
            _context = context;
            _combosHelper = combosHelper;
            _blobHelper = blobHelper;
            _flashMessage = flashMessage;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.Include(x => x.ProductCategories)
                .Include(x => x.ProductImages).ToListAsync());
        }
        [NoDirectAccess]
        public async Task<IActionResult> Create()
        {
            CreateProductViewModel model = new()
            {
                Categories = await _combosHelper.GetComboCategoriesAsync(),
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                string imageId = string.Empty;
                if (model.ImageFile != null)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "products");
                }

                Product product = new()
                {
                    Description = model.Description,
                    Name = model.Name,
                    Price = model.Price,
                    Stock = model.Stock,
                };

                product.ProductCategories = new List<ProductCategory>()
                    {
                        new ProductCategory
                        {
                            Category = await _context.Categories.FindAsync(model.CategoryId)
                        }
                    };

                if (imageId != string.Empty)
                {
                    product.ProductImages = new List<ProductImage>()
                    {
                        new ProductImage { ImageId = imageId }
                    };
                }

                try
                {
                    _context.Add(product);
                    await _context.SaveChangesAsync();
                    _flashMessage.Confirmation("Added Product");
                    return Json(new
                    {
                        isValid = true,
                        html = ModalHelper.RenderRazorViewToString(this, "_ViewAllProducts", _context.Products
                        .Include(p => p.ProductImages)
                        .Include(p => p.ProductCategories)
                        .ThenInclude(pc => pc.Category).ToList())
                    });
                }

                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "There is already a product with the same name.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            model.Categories = await _combosHelper.GetComboCategoriesAsync();
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "Create", model) });


        }
        [NoDirectAccess]
        public async Task<IActionResult> Edit(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            Product product = await _context.Products.FindAsync(Id);
            if (product == null)
            {
                return NotFound();
            }

            EditProductViewModel model = new()
            {
                Description = product.Description,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
                Id = product.Id,
            };
            return View(model);
        }
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Edit(int? Id, EditProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (Id != model.Id)
                {
                    return NotFound();
                }
                try
                {
                    Product product = await _context.Products.FindAsync(model.Id);
                    product.Description = model.Description;
                    product.Name = model.Name;
                    product.Price = model.Price;
                    product.Stock = model.Stock;

                    _context.Products.Update(product);
                    await _context.SaveChangesAsync();
                    _flashMessage.Confirmation("Product Updated");
                    return Json(new
                    {
                        isValid = true,
                        html = ModalHelper.RenderRazorViewToString(this, "_ViewAllProducts", _context.Products
                        .Include(p => p.ProductImages)
                        .Include(p => p.ProductCategories)
                        .ThenInclude(pc => pc.Category).ToList())
                    });


                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "There is already a product with the same name.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "Edit", model) });


        }
        public async Task<IActionResult> Details(int? Id)
        {
            if(Id==null)
            {
                return NotFound();
            }

            Product product = await _context.Products.Include(x => x.ProductCategories)
                .ThenInclude(x => x.Category).Include(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == Id);

            if(product==null)
            {
                return NotFound();
            }
            return View(product);
        }

        [NoDirectAccess]
        public async Task<IActionResult> AddImage(int? Id)
        {
            if(Id==null) 
            {
                return NotFound();
            }
            Product product = await _context.Products.FindAsync(Id);
            if(product==null) { return NotFound(); }
            AddProductImageViewModel model = new()
            {
                ProductId = product.Id,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddImage(AddProductImageViewModel model)
        {
            if (ModelState.IsValid)
            {
                string imageId = string.Empty;
                if(model.ImageFile !=null)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "products");
                }

                Product product = await _context.Products.FindAsync(model.ProductId);
                if(product == null) { return NotFound(); }

                ProductImage productImage = new()
                {
                    Product = product,
                    ImageId = imageId,
                };
                try
                {
                    _context.Add(productImage);
                    await _context.SaveChangesAsync();
                    return Json(new
                    {
                        isValid = true,
                        html = ModalHelper.RenderRazorViewToString(this, "Details", _context.Products
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                    .FirstOrDefaultAsync(p => p.Id == model.ProductId))
                    });

                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
                
            }
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddImage", model) });
        }

        public async Task<IActionResult> DeleteImage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductImage productImage = await _context.ProductImages
                .Include(pi => pi.Product)
                .FirstOrDefaultAsync(pi => pi.Id == id);
            if (productImage == null)
            {
                return NotFound();
            }

            await _blobHelper.DeleteBlobAsync(productImage.ImageFullPath, "products");        
            _context.ProductImages.Remove(productImage);
            await _context.SaveChangesAsync();
            _flashMessage.Info("The image was deleted Successfully.");
            return RedirectToAction(nameof(Details), new { Id = productImage.Product.Id });
        }
        [NoDirectAccess]
        public async Task<IActionResult> AddCategory(int? Id)
        {
            if (Id == null) { return NotFound(); }
            Product product = await _context.Products.Include(x=>x.ProductCategories)
                .ThenInclude(x=>x.Category).FirstOrDefaultAsync(x=>x.Id==Id);
            if (product == null) { return NotFound(); }
            List<Category> categories = product.ProductCategories.Select(x => new Category
            {
                Id = x.Category.Id,
                Name = x.Category.Name,
            }).ToList();
            AddCategoryProductViewModel model = new AddCategoryProductViewModel()
            {
                ProductId = product.Id,
                Categories = await _combosHelper.GetComboCategoriesAsync(categories),
             };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(AddCategoryProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                Product product = await _context.Products.FindAsync(model.ProductId);
                if(product == null) { return NotFound(); }
                ProductCategory productCategory = new()
                {
                    Product=product,
                    Category=await _context.Categories.FindAsync(model.CategoryId),
                };
                try
                {
                    _context.Add(productCategory);
                    await _context.SaveChangesAsync();
                    return Json(new
                    {
                    isValid = true,
                    html = ModalHelper.RenderRazorViewToString(this, "Details", _context.Products
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                    .FirstOrDefaultAsync(p => p.Id == model.ProductId))
                    });               

                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
               
            }
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddCategory", model) });
        }
        public async Task<IActionResult> DeleteCategory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductCategory productCategory = await _context.ProductCategories
                .Include(pc => pc.Product)
                .FirstOrDefaultAsync(pc => pc.Id == id);
            if (productCategory == null)
            {
                return NotFound();
            }

            _context.ProductCategories.Remove(productCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { Id = productCategory.Product.Id });
        }
        [NoDirectAccess]
        public async Task<IActionResult> Delete(int id)
        {
            Product product = await _context.Products
                .Include(p => p.ProductCategories)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            foreach (ProductImage productImage in product.ProductImages)
            {
                await _blobHelper.DeleteBlobAsync(productImage.ImageId, "products");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            _flashMessage.Info("Deleted Item");
            return RedirectToAction(nameof(Index));
        }


    }

}

