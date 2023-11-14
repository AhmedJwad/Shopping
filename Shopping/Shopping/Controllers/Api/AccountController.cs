using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Shopping.Common;
using Shopping.Data;
using Shopping.Data.Entities;
using Shopping.Enum;
using Shopping.Helpers;
using Shopping.Models;
using Shopping.Models.Request;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Shopping.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IConfiguration _configuration;

        public AccountController(DataContext context ,IUserHelper userHelper, IBlobHelper blobHelper,
            IConfiguration configuration, IMailHelper mailHelper)
        {
            _context = context;
            _userHelper = userHelper;
           _blobHelper = blobHelper;
          _mailHelper = mailHelper;
           _configuration = configuration;
        }
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> PostUser([FromBody]UserRequest request)
        {
           
            if(request.ImageId !=null)
            {
               
                request.ImageId = await _blobHelper.UploadBlobAsync(request.ImageArray, "users");
            }
           User user = new User
            {
                UserType = UserType.User,
                City = await _context.Cities.FindAsync(request.CityId),
                UserName=request.Email,
                ImageId=request.ImageId,
                FirstName=request.FirstName,
                LastName=request.LastName,
                PhoneNumber=request.PhoneNumber,
                Email=request.Email,
                Address=request.Address,
                
            };
            var result= await _userHelper.AddUserAsync(user, request.Password);
            if(result.Succeeded)
            {
                await _userHelper.AddUsertoRoleAsync(user, user.UserType.ToString());

            }
            else
            {
                return BadRequest(result.Errors.FirstOrDefault().Description);
            }
            var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            var tokenLink = Url.Action("ConfirmEmail", "Account", new
            {
                userid = user.Id,
                token = myToken
            }, HttpContext.Request.Scheme, _configuration["UrlWEB"]);

            var response = _mailHelper.SendMail(user.FullName, user.Email!,
                $"Shopping - Account Confirmation",
                $"<h1>Shopping - Account Confirmation</h1>" +
                $"<p>To enable the user, please click 'Confirm Email':</p>" +
                $"<b><a href ={tokenLink}>Confirm Email</a></b>");

            if (response.IsSuccess)
            {
                return NoContent();
            }

            return BadRequest(response.Message);

        }

        [HttpPost]
        [Route("CreateToken")]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            var result = await _userHelper.LoginAsync(model);
            if (result.Succeeded)
            {
                var user = await _userHelper.GetUserAsync(model.Username);
                return Ok(BuildToken(user));
            }
            if (result.IsLockedOut)
            {
                return BadRequest("You have exceeded the maximum number of attempts, your account is locked, please try again in 5 minutes.");
            }

            if (result.IsNotAllowed)
            {
                return BadRequest("The user has not been enabled, you must follow the instructions in the email sent to enable the user.");
            }

            return BadRequest("email or password incorrect");
        }

        private Token BuildToken(User user)
        {
            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email!),
                new Claim(ClaimTypes.Role, user.UserType.ToString()),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim("Address", user.Address),
                new Claim("PhoneNumber", user.PhoneNumber),
                new Claim("ImageId", user.ImageId ?? string.Empty),
              

            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]!));
            var credential=new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.Now.AddDays(30);
            var token = new JwtSecurityToken
            (
                issuer:null,
                audience:null,
                claims:claim,
                expires:expiration,
                signingCredentials:credential

            );
           
            return new Token
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration,
                user=user
            };           

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("GetUserByEmail")]
        public async Task<IActionResult> GetUserByEmail()
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            string email = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;
            User user=await _userHelper.GetUserAsync(email);
            if (user==null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        public async Task<IActionResult>PutUser([FromBody]UserRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string email = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;
            if(email==null)
            {
                return BadRequest(ModelState);
            }
            User user = await _userHelper.GetUserAsync(email);
            if(user==null)
            {
                return NotFound();
            }
            City city = await _context.Cities.FindAsync(request.CityId);
            if (city == null)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "Error004"
                });
            }
            string imageId = user.ImageId;
            if(request.ImageArray != null)
            {
                imageId = await _blobHelper.UploadBlobAsync(request.ImageArray, "users");
            }
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber=request.PhoneNumber;
            user.Address = request.Address;
            user.City = city;
            user.ImageId = imageId;

            IdentityResult result = await _userHelper.UpdateUserAsync(user);
            if(!result.Succeeded)
            {
                return BadRequest(result.Errors.FirstOrDefault().Description);
            }
            User updateuser = await _userHelper.GetUserAsync(email);
            return Ok(updateuser);
        }

        [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                string email = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;
                User user = await _userHelper.GetUserAsync(email);
                if (user != null)
                {
                    IdentityResult result = await _userHelper.ChangePasswordAsync(user, model.OldPassword,
                    model.NewPassword);
                    if (result.Succeeded)
                    {
                        return NoContent();
                    }
                    else
                    {
                        return BadRequest(result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    return BadRequest("user no exist");
                }
              
            }else
            {
                return BadRequest(ModelState);
            }       
                     
        }

        [HttpPost]
        [Route("RecoveryPassword")]
        public async Task<IActionResult> RecoveryPassword(RecoverPasswordViewModel model)
        {
            if(ModelState.IsValid)
            {
                User user = await _userHelper.GetUserAsync(model.Email);
                if (user == null)
                {
                    return BadRequest("the email entered does not corrsponding to any user");
                }
                string mytoken=await _userHelper.GeneratePasswordResetTokenAsync(user);
                string link = Url.Action(
                   "ResetPassword",
                   "Account",
                   new { token = mytoken }, protocol: HttpContext.Request.Scheme);
                _mailHelper.SendMail($"{user.FullName} ",
                    model.Email, "Shopping - Password Reset", $"<h1>Shopping - Password Reset</h1>" +
                    $"To set a new password click on the following link:</br></br>" +
                    $"<a href = \"{link}\">Change of password</a>");
                return Ok("Instructions for changing your password have been sent to your email.");
            }
            return BadRequest(model);
        }
         
    
    }
}
