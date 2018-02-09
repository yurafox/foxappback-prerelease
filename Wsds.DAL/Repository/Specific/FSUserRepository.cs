using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Wsds.DAL.Identity;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.DAL.Repository.Specific
{
    public class FSUserRepository:IUserRepository
    {
        public FSUserRepository(UserManager<AppUser> manager)
        {
            UserEngine = manager;
        }

        public UserManager<AppUser> UserEngine { get; }

        public IEnumerable<AppUser> Users => UserEngine.Users.AsNoTracking().ToList();

        public IEnumerable<AppUser> GetUsersByFilter(Func<AppUser, bool> func)
        {
            return UserEngine.Users.AsNoTracking().Where(func);
        }
        public AppUser GetSingleUsersByFilter(Func<AppUser, bool> func)
        {
            return UserEngine.Users.FirstOrDefault(func);
        }

        public async Task<AppUser> GetUserByEmail(string email) => await UserEngine.FindByEmailAsync(email);
        public async Task<AppUser> GetUserById(string id) => await UserEngine.FindByIdAsync(id);
        public async Task<AppUser> GetUserByName(string userName) => await UserEngine.FindByNameAsync(userName);
       

        public async Task<AppUser> CreateUser(AppUser user, string pswd)
        {
            var findedUser=await UserEngine.FindByNameAsync(user.UserName);
            if (findedUser != null) return findedUser;


            var result=await UserEngine.CreateAsync(user,pswd.ToLower());
            if(!result.Succeeded)
                throw new IdentityNotMappedException(result.Errors.First().Description);

            return user;
        }
        public async Task<AppUser> DeleteUser(string id)
        {
            return await StartActionCrud(id, (user) => UserEngine.DeleteAsync(user));
        }
        public async Task<AppUser> UpdateUser(AppUser user)
        {
            return await StartActionCrud(user.Id, (findedUser) =>
            {
                findedUser.UserName = user.UserName;
                findedUser.Email = user.Email;
                return UserEngine.UpdateAsync(findedUser);
            });
        }

        public  IEnumerable<string> UserRoles(string id)
        {
            var usrAsync = GetUserById(id).Result;
            return UserEngine.GetRolesAsync(usrAsync).Result;
           
        }
             

        public async Task<bool> CheckUser(string userName, string pswd)
        {
            var findedUser = await UserEngine.FindByNameAsync(userName.ToLower());
            if (findedUser == null) return false;

            return await UserEngine.CheckPasswordAsync(findedUser, pswd.ToLower());
        }

        #region private behaviors
        private async Task<AppUser> StartActionCrud(string id,Func<AppUser,Task<IdentityResult>> actionFunc)
        {
            var finded = await UserEngine.FindByIdAsync(id);
            if (finded == null)
                throw new ObjectNotFoundException("not found user");

            var result = await actionFunc(finded);

            if (!result.Succeeded)
                throw new Exception(result.Errors.First()?.Description);

            return finded;
        }     
        #endregion
    }
}
