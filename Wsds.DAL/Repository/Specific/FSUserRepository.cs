using System;
using System.Collections.Generic;
using System.Data;
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
using Wsds.DAL.Identity.Exceptions;
using Wsds.DAL.Repository.Abstract;
using Wsds.DAL.Services.Abstract;

namespace Wsds.DAL.Repository.Specific
{
    public class FSUserRepository : IUserRepository
    {
        private const string CompName = "UserRepository";
        private ISmsService _smsService;
        private IAppLocalizationRepository _appLocalization;

        public FSUserRepository(UserManager<AppUser> manager,
                                ISmsService smsService,
                                IAppLocalizationRepository appLocalization)
        {
            UserEngine = manager;
            _smsService = smsService;
            _appLocalization = appLocalization;
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


            var result = await UserEngine.CreateAsync(user, pswd);
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
                return (client?.id == null) ? AbsentUserStrategy() : await OnlyUserAbsentStrategy(client);

            return (client?.id != null) ? await UserInSystemWithOldRegistration(user) 
                                        : await RemoveNotSyncUser(user);
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
                { "currency", (client.id_currency ?? 4).ToString() }, // 4 - UAH
                { "lang", (client.id_lang ?? 1).ToString()} // 1 - RUS
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
        public async Task<bool> CheckUser(string userName, string code)
        {
            var findedUser = await UserEngine.FindByNameAsync(userName.ToLower());
            if (findedUser == null) return false;

           var codeResult = await UserEngine.ChangePhoneNumberAsync(findedUser,findedUser.PhoneNumber,code);
            return codeResult.Succeeded;
        }
        public Client_DTO ToClient(User_DTO user)
        {
            if (user == null)
                return null;

            return new Client_DTO()
            {
                phone = user.phone,
                email = user.email,
                fname = user.fname,
                lname = user.lname,
                id_currency = String.IsNullOrEmpty(user.userSetting["currency"]) ? 4 : // 4 - UAH
                                 Convert.ToInt32(user.userSetting["currency"]),

                id_lang = String.IsNullOrEmpty(user.userSetting["lang"]) ? 1 : // 1 - RUS
                                 Convert.ToInt32(user.userSetting["lang"]),
            };
        }

        public async Task<AppUserManipulationModel> FastUserIdentityCreate(Client_DTO client)
        {
            var data = await OnlyUserAbsentStrategy(client);
            var appUserModel = new AppUserManipulationModel() {Message = data.Item1, Status = data.Item2};
            if (data.Item2 == 2) appUserModel.IdentityUser = await GetUserByName(client.phone);

            return appUserModel;
        }

        #region private behaviors
        private async Task<AppUser> StartActionCrud(string id, Func<AppUser, Task<IdentityResult>> actionFunc)
        {
            var finded = await UserEngine.FindByIdAsync(id);
            if (finded == null)
                throw new DataException("not found user");

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

        private async Task<(string, byte)> RemoveNotSyncUser(AppUser user)
        {
            var resultDel = await UserEngine.DeleteAsync(user);
            if (!resultDel.Succeeded)
                throw new IdentityRemoveException("can't remove application user");

            return ("", 1); // tuple (message:empty status:1)

        }

        private async Task<(string, byte)> OnlyUserAbsentStrategy(Client_DTO client)
        {
            // get card for identity like number
            long card = 0;
            if (!String.IsNullOrEmpty(client.barcode))
            {
                string barCode = client.barcode.Substring(1);
                card = Convert.ToInt64(barCode);
            }
            

            var appUser = new AppUser()
            {
                UserName = client.phone,
                Card = card,
                PhoneNumber = client.phone,
                Email = client.email
            };

            //send sms logic
            try
            {
                // create user in identity
                await UserEngine.CreateAsync(appUser);
                var code = await UserEngine.GenerateChangePhoneNumberTokenAsync(appUser,appUser.UserName);
                await _smsService.SendAuthSmsAsync(appUser.UserName, code);
            }
            catch (OracleException ex)
            {
                return (GetSendSmsErrorLocalizationString,0);
            }

            return (GetSmsWaitLocalizationString(appUser.UserName), 2);
        }
        private (string, byte) UserInSystemStrategy()
        {
            return (GetUserInSystemLocalizationString, 0);
        }
        private async Task<(string, byte)> UserInSystemWithOldRegistration(AppUser user)
        {
            var code = await UserEngine.GenerateChangePhoneNumberTokenAsync(user, user.UserName);

            //send sms logic
            try
            {
                await _smsService.SendAuthSmsAsync(user.UserName, code);
            }
            catch (OracleException ex)
            {
                return (GetSendSmsErrorLocalizationString, 0);
            }

            return (GetSmsWaitLocalizationString(user.UserName), 2);
        }
        #endregion

        // service localize str for message
        private string GetSmsWaitLocalizationString(string phone) => $"{_appLocalization.GetBackLocaleString(CompName, "WaitTempCode")} {phone}";

        private string GetUserInSystemLocalizationString => $"{_appLocalization.GetBackLocaleString(CompName, "AlreadyRegistered")}";

        private string GetSendSmsErrorLocalizationString => $"{_appLocalization.GetBackLocaleString(CompName, "SmsServiceBusy")}";

        #endregion
    }
}
