using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
                if (result.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(Request.Query["ReturnUrl"].First());
                    }

                    return RedirectToAction("Index", "Home");
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "You have exceeded the maximum number of attempts, your account is locked, try again in 5 minutes.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Wrong email or password.");
                }

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
                        model.Countries = await _combosHelper.GetComboCountriesAsync();
                        model.States = await _combosHelper.GetComboStatesAsync(model.CountryId);
                        model.Cities = await _combosHelper.GetComboCitiesAsync(model.StateId);
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
            model.Countries = await _combosHelper.GetComboCountriesAsync();
            model.States = await _combosHelper.GetComboStatesAsync(model.CountryId);
            model.Cities = await _combosHelper.GetComboCitiesAsync(model.StateId);
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

        public async Task<IActionResult> ChangeUser()
        {
            User user = await _userHelper.GetUserAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }
            EditUserViewModel model = new()
            {
                FirstName=user.FirstName,
                LastName=user.LastName,
                Address=user.Address,
                PhoneNumber=user.PhoneNumber,
                ImageId=user.ImageId,
                Id=user.Id,
                Cities=await _combosHelper.GetComboCitiesAsync(user.City.state.Id),
                CityId=user.City.Id,
                Countries=await _combosHelper.GetComboCountriesAsync(),
                CountryId=user.City.state.country.Id,
                States=await _combosHelper.GetComboStatesAsync(user.City.state.country.Id),
                StateId=user.City.state.Id,

            };
            return View(model);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> ChangeUser(EditUserViewModel model)
        {
            if (ModelState.IsValid) 
            {
                string imagrId = string.Empty;

                if(model.ImageFile!=null)
                {
                   
                        imagrId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                    
                }

                User user = await _userHelper.GetUserAsync(User.Identity.Name);
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Address = model.Address;
                user.PhoneNumber = model.PhoneNumber;
                user.ImageId = imagrId;

                await _userHelper.UpdateUserAsync(user);
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user=await _userHelper.GetUserAsync(User.Identity.Name);
                if (user==null)
                {
                    return NotFound();
                }
                if(user!=null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if(result.Succeeded)
                    {
                        return RedirectToAction(nameof(ChangeUser));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }                  
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User no found.");
                }
            }
            return View(model);

        }
    }
}
