using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using Wsds.DAL.Entities;
using Wsds.DAL.Entities.DTO;
using Wsds.DAL.Identity;
using Wsds.DAL.Repository.Abstract;
using Wsds.DAL.Services.Abstract;

namespace Wsds.DAL.Repository.Specific
{
    public class FSUserRepository : IUserRepository
    {
        private ISmsService _smsService;
        public FSUserRepository(UserManager<AppUser> manager, ISmsService smsService)
        {
            UserEngine = manager;
            _smsService = smsService;
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
            var findedUser = await UserEngine.FindByNameAsync(user.UserName);
            if (findedUser != null) return findedUser;


            var result = await UserEngine.CreateAsync(user, pswd.ToLower());
            if (!result.Succeeded)
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

        public async Task<(string, byte)> UserVerifyStrategy(string phone, AppUser user, Client_DTO client)
        {
            if (user == null)
                return (client == null) ? AbsentUserStrategy() : await OnlyUserAbsentStrategy(client);

            return (!String.IsNullOrEmpty(user.PasswordHash))
                ? UserInSystemStrategy()
                : await UserInSystemWithOldRegistration(user);
        }

        public bool VerifyUserPhoneInputData(string phone)
        {
            if (String.IsNullOrEmpty(phone))
                return false;

            var match = Regex.Match(phone, "^380[0-9]{9}$");
            return match.Success;
        }

        public IEnumerable<string> UserRoles(string id)
        {
            var usrAsync = GetUserById(id).Result;
            return UserEngine.GetRolesAsync(usrAsync).Result;

        }


        public User_DTO Swap(Client_DTO client, Func<string, string> encrypt)
        {
            var dict = new Dictionary<string, string>
            {
                { "currency", (client.id_currency ?? 0).ToString() },
                { "lang", (client.id_lang ?? 1).ToString()}
            };


            User_DTO user = new User_DTO()
            {
                email = client.email,
                appKey = (client.appKey != null) ? encrypt(client.appKey) : "",
                //login = client.login,
                name = client.name,
                phone = client.phone,
                fname = client.fname,
                lname = client.lname,
                userSetting = dict
            };

            return user;
        }


        public async Task<bool> CheckUser(string userName, string pswd)
        {
            var findedUser = await UserEngine.FindByNameAsync(userName.ToLower());
            if (findedUser == null) return false;

            return await UserEngine.CheckPasswordAsync(findedUser, pswd);
        }

        #region private behaviors
        private async Task<AppUser> StartActionCrud(string id, Func<AppUser, Task<IdentityResult>> actionFunc)
        {
            var finded = await UserEngine.FindByIdAsync(id);
            if (finded == null)
                throw new ObjectNotFoundException("not found user");

            var result = await actionFunc(finded);

            if (!result.Succeeded)
                throw new Exception(result.Errors.First()?.Description);

            return finded;
        }

        #region UserValidateStrategy
        private (string, byte) AbsentUserStrategy()
        {
            return ("", 1); // tuple (message:empty status:1)
        }
        private async Task<(string, byte)> OnlyUserAbsentStrategy(Client_DTO client)
        {
            var appUser = new AppUser() { UserName = client.phone, Card = (long)client.id, PhoneNumber = client.phone ,Email = client.email};
            // generate random temp password for sms
            var tempPswd = _smsService.GetAuthTempPswd(8);

            //send sms logic
            try
            {
                await _smsService.SendAuthSmsAsync(appUser.UserName, tempPswd);
            }
            catch (OracleException ex)
            {
                //TODO: maybe will create logging logic 
                return (GetSendSmsErrorLocalizationString,0);
            }

            // create user in identity
            await UserEngine.CreateAsync(appUser, tempPswd);
            return (GetSmsWaitLocalizationString, 2);
        }
        private (string, byte) UserInSystemStrategy()
        {
            return (GetUserInSystemLocalizationString, 0);
        }
        private async Task<(string, byte)> UserInSystemWithOldRegistration(AppUser user)
        {
            // generate random temp password for sms
            var tempPswd = _smsService.GetAuthTempPswd(8);

            //send sms logic
            try
            {
                await _smsService.SendAuthSmsAsync(user.UserName, tempPswd);
            }
            catch (OracleException ex)
            {
                //TODO: maybe will create logging logic 
                return (GetSendSmsErrorLocalizationString, 0);
            }
       
            // update passwd by temp password
            await UserEngine.AddPasswordAsync(user, tempPswd);
            return (GetSmsWaitLocalizationString, 2);
        }
        #endregion

        // service localize str for message
        private string GetSmsWaitLocalizationString
        {
            get
            {
                // TODO:change after localization logic will be Ok 
                return "Ожидайте временного пароля в SMS сообщении. Обязательно измените пароль " +
                       "в настройках после авторизации";
            }
        }
        private string GetUserInSystemLocalizationString
        {
            get
            {
                // TODO:change after localization logic will be Ok 
                return "Вы уже зарегистрированы в системе";
            }
        }
        private string GetSendSmsErrorLocalizationString
        {
            get
            {
                // TODO:change after localization logic will be Ok 
                return "Сервис sms занят. Ожидайте смс";
            }
        }
        #endregion
    }
}
