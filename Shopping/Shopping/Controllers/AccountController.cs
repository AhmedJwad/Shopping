using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Shopping.Common;
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
        private readonly IMailHelper _mailHelper;

        public AccountController(DataContext context, IUserHelper userHelper, ICombosHelper combosHelper,
            IBlobHelper blobHelper, IMailHelper mailHelper)
        {
             _context = context;
            _userHelper = userHelper;
           _combosHelper = combosHelper;
            _blobHelper = blobHelper;
            _mailHelper = mailHelper;
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
                else if (result.IsNotAllowed)
                {
                    ModelState.AddModelError(string.Empty, "The user has not been enabled, you must follow the instructions in the email sent to enable the user.");
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

                string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                string tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                Response response = _mailHelper.SendMail(
                    $"{model.FirstName} {model.LastName}",
                    model.Username,
                    "Shopping - Email Confirmation",
                    $"<h1>Shopping - Email Confirmation</h1>" +
                        $"To enable the user please click on the following link: " +
                        $"<p><a href = \"{tokenLink}\">Confirm Email</a></p>");
                if (response.IsSuccess)
                {
                    ViewBag.Message = "Instructions to enable the user have been sent to the email.";
                    return View(model);
                }

                ModelState.AddModelError(string.Empty, response.Message);
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
                if (model.OldPassword == model.NewPassword)
                {
                    ModelState.AddModelError(string.Empty, "You must enter a different password.");
                    return View(model);
                }
                User? user=await _userHelper.GetUserAsync(User.Identity.Name);
                if (user==null)
                {
                    return NotFound();
                }
                if(user!=null)
                {
                   IdentityResult? result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
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

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
             {
                return NotFound();
            }
            User user = await _userHelper.GetUserAsync(new Guid(userId));
            if (user==null)
            {
                return NotFound();
            }

            IdentityResult result = await _userHelper.ConfirmEmailAsync(user, token);
            if(!result.Succeeded)
            {
                return NotFound();
            }
            return View();
               
        }
        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserAsync(model.Email);
                if (user==null) 
                {
                    ModelState.AddModelError(string.Empty, "The email does not correspond to any registered user.");
                    return View(model);

                }
                string myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
                string link = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { token = myToken }, protocol: HttpContext.Request.Scheme);
                _mailHelper.SendMail(
                    $"{user.FullName}",
                    model.Email,
                    "Shopping - Password recovery",
                    $"<h1>Shopping - Password recovery</h1>" +
                    $"To recover your password click on the following link:" +
                    $"<p><a href = \"{link}\">Reset Password</a></p>");
                ViewBag.Message = "Instructions to recover your password have been sent to your email.";
                return View();

            }
            return View(model);
        }
        public IActionResult ResetPassword(string token)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            User user = await _userHelper.GetUserAsync(model.UserName);
            if(user != null)
            {
                IdentityResult result = await _userHelper.ResetPasswordAsync(user,
                    model.Token, model.Password);
                if(result.Succeeded)
                {
                    ViewBag.Message = "Password changed successfully.";
                    return View();

                }
            }
            ViewBag.Message = "Error changing password.";
            return View(model); 

        }


    }
}
