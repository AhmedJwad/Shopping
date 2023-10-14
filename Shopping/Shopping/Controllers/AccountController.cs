using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Data;
using Shopping.Data.Entities;
using Shopping.Enum;
using Shopping.Helpers;
using Shopping.Migrations;
using Shopping.Models;

namespace Shopping.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IBlobHelper _blobHelper;

        public AccountController(DataContext context, IUserHelper userHelper, ICombosHelper combosHelper,
            IBlobHelper blobHelper)
        {
             _context = context;
            _userHelper = userHelper;
           _combosHelper = combosHelper;
            _blobHelper = blobHelper;
        }
        public IActionResult Login()
        {
           if(User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult>Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
              Microsoft.AspNetCore.Identity.SignInResult result = await _userHelper.LoginAsync(model);

                if(result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Wrong email or password.");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult NotAuthorized()

        {
            return View();

        }

        public async Task<IActionResult>Register()
        {
            AddUserViewModel model = new AddUserViewModel
            {
                Id=Guid.Empty.ToString(),
                Countries=await _combosHelper.GetComboCountriesAsync(),
                States=await _combosHelper.GetComboStatesAsync(0),
                Cities=await _combosHelper.GetComboCitiesAsync(0),
                UserType=UserType.User,

            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
               string imageId= string.Empty;               

                if (model.ImageFile != null)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                }
               
                    User user = await _userHelper.AddUserAsync(model, imageId);

                    if (user == null)
                    {
                        ModelState.AddModelError(string.Empty, "This email is already being used.");
                        return View(model);
                    }                             

                LoginViewModel loginvewmodel = new LoginViewModel
                {
                    Password = model.Password,
                    Username = model.Username,
                    RememberMe = false,
                };
                var result2 = await _userHelper.LoginAsync(loginvewmodel);
                if (result2.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }               

            }
            return View(model);
        }

        public JsonResult GetStates(int countryId)
        {
            Country country = _context.Countries
                .Include(c => c.States)
                .FirstOrDefault(c => c.Id == countryId);
            if (country == null)
            {
                return null;
            }

            return Json(country.States.OrderBy(d => d.Name));
        }

        public JsonResult GetCities(int stateId)
        {
            State state = _context.states
                .Include(s => s.Cities)
                .FirstOrDefault(s => s.Id == stateId);
            if (state == null)
            {
                return null;
            }

            return Json(state.Cities.OrderBy(c => c.Name));
        }


    }
}
