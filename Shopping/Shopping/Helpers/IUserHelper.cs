﻿using Microsoft.AspNetCore.Identity;
using Shopping.Data.Entities;
using Shopping.Models;

namespace Shopping.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserAsync(string email);
        Task<IdentityResult> AddUserAsync(User user , string password);
        Task CheckRoleAsync(string roleName);
        Task AddUsertoRoleAsync (User user, string roleName);
        Task<bool> isUserinRoleAsync(User user, string roleName);
        Task<SignInResult> LoginAsync(LoginViewModel model);
        Task LogoutAsync();

    }

}
