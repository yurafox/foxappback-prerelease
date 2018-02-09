using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Wsds.DAL.Identity;

namespace Wsds.DAL.Repository.Abstract
{
    public interface IUserRepository
    {
        IEnumerable<AppUser> Users { get; }
        IEnumerable<AppUser> GetUsersByFilter(Func<AppUser,bool> func);
        AppUser GetSingleUsersByFilter(Func<AppUser, bool> func);

        Task<AppUser> GetUserById(string id);
        Task<AppUser> GetUserByName(string userName);
        Task<AppUser> GetUserByEmail(string email);

        UserManager<AppUser> UserEngine { get; }
        Task<bool> CheckUser(string userName, string pswd);

        Task<AppUser> CreateUser(AppUser user, string pswd);
        Task<AppUser> DeleteUser(string id);
        Task<AppUser> UpdateUser(AppUser user);

        IEnumerable<string> UserRoles(string id);


    }
}
