using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Common;
using Shopping.Data;
using Shopping.Data.Entities;
using Shopping.Enum;
using Shopping.Helpers;

using Shopping.Models;

namespace Shopping.Controllers
{
    [Authorize(Roles ="Admin")]
    public class UsersController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IMailHelper _mailHelper;

        public UsersController(DataContext context , IUserHelper userHelper, IBlobHelper blobHelper , 
            ICombosHelper combosHelper, IMailHelper mailHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _blobHelper = blobHelper;
            _combosHelper = combosHelper;
            _mailHelper = mailHelper;
        }
        public async Task<IActionResult> Index()
        {
            return View( await _context.Users.Include(x=>x.City)
                .ThenInclude(x=>x.state).ThenInclude(x=>x.country).ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            AddUserViewModel model = new AddUserViewModel
            {
                Id=Guid.Empty.ToString(),
                Countries = await _combosHelper.GetComboCountriesAsync(),
                States = await _combosHelper.GetComboStatesAsync(0),
                Cities = await _combosHelper.GetComboCitiesAsync(0),
                UserType = UserType.Admin,

            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                string imageId = string.Empty;
                if(model.ImageFile != null)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                }
                User user = await _userHelper.AddUserAsync(model, imageId);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "This email is already being used");
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
                        $"To enable the user please click on the following link:, " +
                        $"<p><a href = \"{tokenLink}\">Confirm Email</a></p>");
                if (response.IsSuccess)
                {
                    ViewBag.Message = "Instructions to enable the user have been sent to the email.";
                    return View(model);
                }

                ModelState.AddModelError(string.Empty, response.Message);

            }
            return View(model);
        }

        public JsonResult? GetStates(int countryId)
        {
            Country? country = _context.Countries
                .Include(c => c.States)
                .FirstOrDefault(c => c.Id == countryId);
            if (country == null)
            {
                return null;
            }

            return Json(country.States.OrderBy(d => d.Name));
        }

        public JsonResult? GetCities(int stateId)
        {
            State? state = _context.states
                .Include(s => s.Cities)
                .FirstOrDefault(s => s.Id == stateId);
            if (state == null)
            {
                return null;
            }

            return Json(state.Cities.OrderBy(c => c.Name));
        }
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel());
        }
    }

}

