using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shopping.Data;
using Shopping.Data.Entities;

using Shopping.Models;

namespace Shopping.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;

        public UserHelper(DataContext context ,UserManager<User> userManager, RoleManager<IdentityRole> roleManager , 
            SignInManager<User> signInManager)
        {
           _context = context;
            _userManager = userManager;
           _roleManager = roleManager;
           _signInManager = signInManager;
        }
        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
          return await _userManager.CreateAsync(user , password);
        }

        public async Task<User> AddUserAsync(AddUserViewModel model, string imageID)
        {
            User user = new User
            {
                Address = model.Address,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Username,
                UserName = model.Username,
                ImageId = imageID,
                City = await _context.Cities.FindAsync(model.CityId),
                UserType = model.UserType,
                PhoneNumber = model.PhoneNumber,
            };
            IdentityResult result = await _userManager.CreateAsync(user, model.Password);
            if (result != IdentityResult.Success)
            {
                return null;
            }
            User newuse = await GetUserAsync(model.Username);
            await AddUsertoRoleAsync(newuse , user.UserType.ToString());
            return newuse;
        }

       

        public async Task AddUsertoRoleAsync(User user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);          

        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword)
        {
           return await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        }

        public async Task CheckRoleAsync(string roleName)
        {
           bool roleExist=await _roleManager.RoleExistsAsync(roleName);
            if(!roleExist)
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = roleName,
                });
            }
        }

       
        public async Task<User> GetUserAsync(string email)
        {
            return await _context.Users.Include(x => x.City).ThenInclude(x=>x.state)
                .ThenInclude(x=>x.country)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetUserAsync(Guid userId)
        {
            return await _context.Users.Include(x => x.City)
               .ThenInclude(x => x.state).ThenInclude(x => x.country)
               .FirstOrDefaultAsync(x => x.Id == userId.ToString());
        }

        public async Task<bool> isUserinRoleAsync(User user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            return await _signInManager.PasswordSignInAsync
             (
                model.Username,
                model.Password,
                model.RememberMe,
                true
             );
                
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();

        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await _userManager.UpdateAsync(user);
        }
        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string password)
        {
           return await _userManager.ResetPasswordAsync(user, token, password);
        }
    }
}
