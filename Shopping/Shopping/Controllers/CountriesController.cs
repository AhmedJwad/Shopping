using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping.Data;
using Shopping.Data.Entities;
using Shopping.Helpers;
using Shopping.Models;
using Vereyon.Web;
using static Shopping.Helpers.ModalHelper;

namespace Shopping.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CountriesController : Controller
    {
        private readonly DataContext _context;
        private readonly IFlashMessage _flashMessage;

        public CountriesController(DataContext context, IFlashMessage flashMessage)
        {
            _context = context;
            _flashMessage = flashMessage;
        }

		

		// GET: Countries
		public async Task<IActionResult> Index()
        {
            return View(await _context.Countries.Include(x=>x.States).ThenInclude(x=>x.Cities).ToListAsync());
                         
        }

        // GET: Countries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Countries == null)
            {
                return NotFound();
            }

            var country = await _context.Countries.Include(x=>x.States).ThenInclude(x=>x.Cities)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }
        public async Task<IActionResult> DetailsCity(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            City city = await _context.Cities.Include(x => x.state).ThenInclude(x=>x.country)
                .FirstOrDefaultAsync(x => x.Id == Id);
            if (city == null)
            {
                return NotFound();
            }
            return View(city);
        }
        [NoDirectAccess]
        public async Task<IActionResult> AddCity(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            State state = await _context.states.FindAsync(Id);
            if (state == null)
            {
                return NotFound();
            }
            CityViewModel model = new()
            {
                StateId =state.Id,
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCity(CityViewModel model)
        {
            if (ModelState.IsValid)
            {
                State state = await _context.states.FindAsync(model.StateId);
                City city = new()
                {
                    state = state,
                    Name = model.Name
                };
                _context.Add(city);
                try
                {
                    await _context.SaveChangesAsync();
                    state = await _context.states
                        .Include(s => s.Cities)
                        .FirstOrDefaultAsync(c => c.Id == model.StateId);
                    return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAllCities", state) });
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        _flashMessage.Danger("There is already a city with the same name.");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                }
            }

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "CreateCity", model) });
        }
        [NoDirectAccess]
        public async Task<IActionResult> EditCity(int? id)
        {
            if (id == null || _context.Cities == null)
            {
                return NotFound();
            }

            City city = await _context.Cities.Include(x => x.state).FirstOrDefaultAsync(x => x.Id == id);
            if (city == null)
            {
                return NotFound();
            }
            CityViewModel model = new()
            {
                Id = city.Id,
                StateId = city.state.Id,
                Name = city.Name,
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCity(int id, CityViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    City city = new()
                    {
                        Id = model.Id,
                        Name = model.Name,
                    };
                    _context.Update(city);
                    await _context.SaveChangesAsync();
                    State state = await _context.states
                .Include(s => s.Cities)
                .FirstOrDefaultAsync(c => c.Id == model.StateId);
                    _flashMessage.Confirmation("Registro actualizado.");
                    return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAllCities", state) });

                }
                catch (DbUpdateException dbUpdateException)
                {

                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "There is already a category with the same name.");
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
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "EditCity", model) });
        }
        public async Task<IActionResult> DeleteCity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            City city = await _context.Cities
                .Include(c => c.state)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (city == null)
            {
                return NotFound();
            }

            try
            {
                _context.Cities.Remove(city);
                await _context.SaveChangesAsync();
            }
            catch
            {
                _flashMessage.Danger("The city cannot be deleted because it has related records.");
            }

            _flashMessage.Info("Deleted City");
            return RedirectToAction(nameof(DetailsState), new { Id = city.state.Id });
        }

        public async Task<IActionResult> DetailsState(int? Id)
        {
            if(Id==null)
            {
                return NotFound();
            }
            State state = await _context.states.Include(x => x.country).Include(x => x.Cities)
                .FirstOrDefaultAsync(x => x.Id == Id);
            if(state ==null)
            {
                return NotFound();
            }
            return View(state);
        }
        [NoDirectAccess]
        public async Task <IActionResult> AddState(int Id)
        {
            if(Id == null)
            {
                return NotFound();
            }
            Country country = await _context.Countries.FindAsync(Id);
            if(country == null)
            {
                return NotFound();
            }
            StateViewModel model = new()
            {
                CountryId = country.Id,
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddState(StateViewModel model)
        {
            if (ModelState.IsValid)
            {               
                try
                {
                    State state = new()
                    {
                        Cities = new List<City>(),
                        country = await _context.Countries.FindAsync(model.CountryId),
                        Name=model.Name,
                    };
                    _context.Add(state);
                    await _context.SaveChangesAsync();
                    Country country = await _context.Countries
                    .Include(c => c.States)
                    .ThenInclude(s => s.Cities)
                    .FirstOrDefaultAsync(c => c.Id == model.CountryId);
                    _flashMessage.Info("Added State");
                    return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAllStates", country) });

                }
                catch (DbUpdateException dbUpdateException)
                {

                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "There is already a State with the same name.");
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
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddState", model) });
        }
        [NoDirectAccess]
        public async Task<IActionResult> EditState(int? id)
        {
            if (id == null || _context.Countries == null)
            {
                return NotFound();
            }

            State state = await _context.states.Include(x=>x.country).FirstOrDefaultAsync(x=>x.Id==id);
            if (state == null)
            {
                return NotFound();
            }
            StateViewModel model = new()
            {
                Id=state.Id,
                CountryId=state.country.Id,
                Name=state.Name,
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditState(int id, StateViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    State state = new()
                    {
                        Id=model.Id,
                        Name=model.Name,
                    };
                    _context.Update(state);
                    Country country = await _context.Countries
                    .Include(c => c.States)
                    .ThenInclude(s => s.Cities)
                    .FirstOrDefaultAsync(c => c.Id == model.CountryId);
                    await _context.SaveChangesAsync();
                    return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAllStates", country) });
                }
                catch (DbUpdateException dbUpdateException)
                {

                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "There is already a category with the same name.");
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
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "EditState", model) });
        }

        public async Task<IActionResult> DeleteState(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            State state = await _context.states
                .Include(s => s.country)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (state == null)
            {
                return NotFound();
            }

            try
            {
                _context.states.Remove(state);
                await _context.SaveChangesAsync();
                _flashMessage.Info("Deleted Item.");
            }
            catch
            {
                _flashMessage.Danger("You cannot delete the state/department because it has related records.");
            }

            return RedirectToAction(nameof(Details), new { Id = state.country.Id });
        }

       
      


        [NoDirectAccess]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Country country = await _context.Countries.FirstOrDefaultAsync(c => c.Id == id);
            if (country == null)
            {
                return NotFound();
            }

            try
            {
                _context.Countries.Remove(country);
                await _context.SaveChangesAsync();
                _flashMessage.Info("Deleted Item");
            }
            catch
            {
                _flashMessage.Danger("The country cannot be deleted because it has related records.");
            }

            return RedirectToAction(nameof(Index));
        }

        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                return View(new Country());
            }
            else
            {
                Country country = await _context.Countries.FindAsync(id);
                if (country == null)
                {
                    return NotFound();
                }

                return View(country);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, Country country)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0) //Insert
                    {
                        _context.Add(country);
                        await _context.SaveChangesAsync();
                        _flashMessage.Info("Added Country");
                    }
                    else //Update
                    {
                        _context.Update(country);
                        await _context.SaveChangesAsync();
                        _flashMessage.Info("Update Country.");
                    }
                    return Json(new
                    {
                        isValid = true,
                        html = ModalHelper.RenderRazorViewToString(
                            this,
                            "_ViewAll",
                            _context.Countries
                                .Include(c => c.States)
                                .ThenInclude(s => s.Cities)
                                .ToList())
                    });
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        _flashMessage.Danger("Ya existe un país con el mismo nombre.");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                }
            }

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddOrEdit", country) });
        }


    }
}
