using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Wsds.DAL.Entities;
using Wsds.DAL.Entities.DTO;
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
        Task<(string, byte)> UserVerifyStrategy(string phone, AppUser user, Client_DTO client);
        bool VerifyUserPhoneInputData(string phone);
        IEnumerable<string> UserRoles(string id);
        User_DTO Swap(Client_DTO client,Func<string,string> encrypt);
        Client_DTO ToClient(User_DTO user);
        Task<AppUserManipulationModel> FastUserIdentityCreate(Client_DTO client);
    }
}
