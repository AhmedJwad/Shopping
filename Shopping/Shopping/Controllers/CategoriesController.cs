using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Data;
using Shopping.Data.Entities;
using Shopping.Helpers;
using System.Diagnostics.Metrics;
using Vereyon.Web;
using static Shopping.Helpers.ModalHelper;

namespace Shopping.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly DataContext _Context;
        private readonly IFlashMessage _flashMessage;

        public CategoriesController(DataContext Context, IFlashMessage flashMessage )
        {
           _Context = Context;
           _flashMessage = flashMessage;
        }
        public async Task<IActionResult>Index()
        {
            return View(await _Context.Categories.Include(x=>x.ProductCategories).ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _Context.Categories == null)
            {
                return NotFound();
            }

            var category = await _Context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if(ModelState.IsValid)
            {
               
                try
                {
                    _Context.Add(category);
                    await _Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {

                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        _flashMessage.Danger( "There is already a country with the same name.");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message);
                    }
;
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                }


            }
            return View(category);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _Context.Categories == null)
            {
                return NotFound();
            }

            var country = await _Context.Categories.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                   _Context.Update(category);
                    await _Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {

                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        _flashMessage.Danger("There is already a country with the same name.");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message);
                    }
;
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                }


            }
            return View(category);
        }
     
        [NoDirectAccess]
        public async Task<IActionResult> Delete(int? id)
        {
            Category category = await _Context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            try
            {
               _Context.Categories.Remove(category);
                await _Context.SaveChangesAsync();
                _flashMessage.Info("Deleted Item");
            }
            catch
            {
                _flashMessage.Danger("The category cannot be deleted because it has related records.");
            }

            return RedirectToAction(nameof(Index));
        }

        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                return View(new Category());
            }
            else
            {
                Category category = await _Context.Categories.FindAsync(id);
                if (category == null)
                {
                    return NotFound();
                }

                return View(category);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, Category category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0) //Insert
                    {
                       _Context.Add(category);
                        await _Context.SaveChangesAsync();
                        _flashMessage.Info("Category Added");
                    }
                    else //Update
                    {
                        _Context.Update(category);
                        await _Context.SaveChangesAsync();
                        _flashMessage.Info("catagory updates");
                    }
                }

                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        _flashMessage.Danger("There is already a category with the same name.");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message);
                    }
                    return View(category);
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                    return View(category);
                }

                return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAll", _Context.Categories.Include(c => c.ProductCategories).ToList()) });

            }

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddOrEdit", category) });
        }


    }
}
