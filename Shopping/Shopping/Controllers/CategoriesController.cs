using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Data;
using Shopping.Data.Entities;
using System.Diagnostics.Metrics;

namespace Shopping.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly DataContext _Context;

        public CategoriesController(DataContext Context)
        {
           _Context = Context;
        }
        public async Task<IActionResult>Index()
        {
            return View(await _Context.Categories.ToListAsync());
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
                        ModelState.AddModelError(string.Empty, "There is already a country with the same name.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
;
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
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
                        ModelState.AddModelError(string.Empty, "There is already a country with the same name.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
;
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }


            }
            return View(category);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _Context.Categories == null)
            {
                return NotFound();
            }

            var category = await _Context.Categories
                .FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
           
            Category category = await _Context.Categories.FindAsync(id);            
           _Context.Categories.Remove(category);         
            await _Context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
