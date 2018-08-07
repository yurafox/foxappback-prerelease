using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Wsds.DAL.Entities.DTO;
using Wsds.DAL.Infrastructure.Facade;
using Wsds.WebApp.Auth;
using Wsds.WebApp.Auth.Protection;
using Wsds.WebApp.Filters;
using Wsds.WebApp.Models;
using Wsds.WebApp.WebExtensions;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AccountController : Controller
    {
        private const string CompName = "AccountController";
        private AccountUserFacade _account;
        private ICrypto _crypto;

        public AccountController(AccountUserFacade account, ICrypto crypto)
        {
            _account = account;
            _crypto = crypto;
        }

        [HttpPost("login")]
        public async Task Login([FromBody] AuthModel auth)
        {
            var resultMessage = $"{_account.Locale.GetBackLocaleString(CompName, "LoginInvalidAuth")}";
            if (ModelState.IsValid)
            {
                var resultCompare = await _account.Users.CheckUser(auth.Phone, auth.Password);
                if (!resultCompare) resultMessage = $"{_account.Locale.GetBackLocaleString(CompName, "LoginIncorrectAuth")}";
                else
                {
                    var user = await _account.Users.UserEngine.FindByNameAsync(auth.Phone.ToLower());
                    var roles = await _account.Users.UserEngine.GetRolesAsync(user);
                    //var roles = await _userRepository.UserEngine.GetRolesAsync(user);

                    // get clients
                    var client = _account.Clients.GetClientByPhone(user.UserName);
                    if (client?.id != null)
                    {
                        // get token
                        var jToken = AuthOpt.GetToken(user,client.id,roles);
                  
                        var responseObj = new
                        {
                            token = jToken,
                            user = _account.Users.Swap(client,_crypto.Encrypt)
                        };

                        await Response.WriteAsync(JsonConvert.SerializeObject(responseObj,
                            new JsonSerializerSettings {Formatting = Formatting.Indented}));
                        return;
                    }
                    resultMessage = $"{_account.Locale.GetBackLocaleString(CompName, "LoginNotClient")}";
                }
            }

            await Response.WriteAsync(JsonConvert.SerializeObject(new {message= resultMessage },
                new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }

        [Authorize]
        [HttpGet]
        [PullToken]
        public async Task Get()
        {
            var tokenModel = HttpContext.GetTokenModel();
            if (tokenModel != null)
            {
                var client = _account.Clients.GetClientByPhone(tokenModel.Phone);
                if (client?.id != null)
                {
                    var user = _account.Users.Swap(client,_crypto.Encrypt);

                    await Response.WriteAsync(JsonConvert.SerializeObject(user,
                        new JsonSerializerSettings {Formatting = Formatting.Indented}));

                    return;
                }
            }

            Response.StatusCode = 404;
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] VerifyModel vm)
        {
            if (!_account.Users.VerifyUserPhoneInputData(vm.Phone))
                return Json(new {message = $"{_account.Locale.GetBackLocaleString(CompName, "VerifyPhoneNotValid")}", status = 0});

            // get user and client
            var user = await _account.Users.GetUserByName(vm.Phone);
            var client = _account.Clients.GetClientByPhone(vm.Phone);
            var data = await _account.Users.UserVerifyStrategy(vm.Phone, user, client);

            // send json
            return Json(new {message = data.Item1, status = data.Item2});
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] User_DTO user)
        {
            Response.StatusCode = 200;
  
            if (!ModelState.IsValid)
                return Json(new {message = $"{_account.Locale.GetBackLocaleString(CompName, "CreateAccountNotValid")}", status = 0});
            

            var findedUser = await _account.Users.GetUserByName(user.phone);
            var findedClient = _account.Clients.GetClientByPhone(user.phone);

            if (findedUser != null || findedClient?.id != null)
                return Json(new { message = $"{_account.Locale.GetBackLocaleString(CompName, "CreateAccountExists")}", status = 0});
            

            // create client
            var client = _account.Users.ToClient(user);
            var clientCreated=_account.Clients.CreateOrUpdateClient(client);

            if(clientCreated?.id == null)
                return Json(new { message = $"{_account.Locale.GetBackLocaleString(CompName, "CreateAccountError—reating")}", status = 0 });
            

            // create identity user
            var identityWrapper = await _account.Users.FastUserIdentityCreate(clientCreated);
            if (identityWrapper.IdentityUser != null && identityWrapper.Status != 0)
            {
                Response.StatusCode = 201;
                return Json(new { content = _account.Users.Swap(clientCreated, _crypto.Encrypt), message=identityWrapper.Message,status = identityWrapper.Status });
            }

            
            return Json(new { message = $"{_account.Locale.GetBackLocaleString(CompName, "CreateAccountError—reating")}", status = 0 });

        }

        [Authorize]
        [HttpPut]
        [PullToken]
        public async Task<IActionResult> EditUser([FromBody] User_DTO user)
        {
            Response.StatusCode = 200;
            var tokenModel = HttpContext.GetTokenModel();
            if (tokenModel != null && user.phone == tokenModel.Phone)
            {          

                if (!ModelState.IsValid){
                    return Json(new { message = $"{_account.Locale.GetBackLocaleString(CompName, "CreateAccountNotValid")}", status = 0 });
                }


                var findedUser = await _account.Users.GetUserByName(user.phone);
                var findedClient = _account.Clients.GetClientByPhone(user.phone);

                if (findedUser == null || findedClient?.id == null)
                    return Json(new { message = $"{_account.Locale.GetBackLocaleString(CompName, "EditAccountErrorEdit")}", status = 0 });

                // update client
                var client = _account.Users.ToClient(user);
                client.barcode = findedClient.barcode;

                var clientCreated = _account.Clients.CreateOrUpdateClient(client);

                if (clientCreated?.id == null)
                    return Json(new { message = $"{_account.Locale.GetBackLocaleString(CompName, "EditAccountErrorEdit")}", status = 0 });

                // update user
                findedUser.Email = clientCreated.email;
                findedUser.NormalizedEmail = clientCreated.email.ToUpper();
                var identityUser = await _account.Users.UserEngine.UpdateAsync(findedUser);

                if (identityUser == null)
                    return Json(new { message = $"{_account.Locale.GetBackLocaleString(CompName, "EditAccountIdentityError")}", status = 0 });

                return Json(new
                { content = _account.Users.Swap(clientCreated, _crypto.Encrypt),
                  message = $"{_account.Locale.GetBackLocaleString(CompName, "EditAccountUpdateSuccess")}",
                  status = 2 }
                );
            }

            Response.StatusCode = 401;
            return Json(new { message = $"{_account.Locale.GetBackLocaleString(CompName, "EditAccountAuthError")}", status = 0 });
        }

        [Authorize]
        [HttpPost("changePass")]
        [PullToken]
        public async Task<IActionResult> ChangePassword([FromBody] PasswdModel passwd)
        {
            Response.StatusCode = 200;
            var tokenModel = HttpContext.GetTokenModel();
            if (tokenModel != null)
            {
                if (!ModelState.IsValid){
                    return Json(new { message = $"{_account.Locale.GetBackLocaleString(CompName, "ChangePasswordNotValid")}", status = 0 });
                }

                var resultCompare = await _account.Users.CheckUser(tokenModel.Phone, passwd.Password);
                if (!resultCompare) return Json(new { message = $"{_account.Locale.GetBackLocaleString(CompName, "ChangePasswordFailedLink")}", status = 0 });

                var identityUser = await _account.Users.GetUserByName(tokenModel.Phone);
                var result = await _account.Users.UserEngine.ChangePasswordAsync(identityUser, passwd.Password, passwd.NewPassword);
                if(!result.Succeeded) return Json(new { message = $"{_account.Locale.GetBackLocaleString(CompName, "ChangePasswordFailedChange")}", status = 0 });

                return Json(new { message = $"{_account.Locale.GetBackLocaleString(CompName, "ChangePasswordSuccessChange")}", status = 2 });
            }

            Response.StatusCode = 401;
            return Json(new { message = $"{_account.Locale.GetBackLocaleString(CompName, "ChangePasswordErrorAuth")}", status = 0 });
        }

        [Authorize(Roles = "retail")]
        [HttpPost("key")]
        public IActionResult CreateAppKey([FromBody] AppKeys_DTO key)
        {
            if(!ModelState.IsValid)
                return BadRequest();

            var createdKey = _account.Clients.CreateApplicationKey(key);
            return (createdKey != null) ? Ok(createdKey) : (IActionResult) BadRequest();
        }
    }
}