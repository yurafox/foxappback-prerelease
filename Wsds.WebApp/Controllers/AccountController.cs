using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
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
            var resultMessage = "Invalid Authentication Data";
            if (ModelState.IsValid)
            {
                var resultCompare = await _account.Users.CheckUser(auth.Phone, auth.Password);
                if (!resultCompare) resultMessage = "Login or Password incorrect";
                else
                {
                    var user = await _account.Users.UserEngine.FindByNameAsync(auth.Phone.ToLower());
                    var roles = await _account.Users.UserEngine.GetRolesAsync(user);
                    //TODO: check this role method when we will be create admin panel
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
                    resultMessage = "Can't find client";
                }
            }

            Response.StatusCode = 400;
            await Response.WriteAsync(resultMessage);
        }

        [Authorize]
        [HttpGet]
        [PullToken]
        public async Task Get()
        {
            var tokenModel = HttpContext.GeTokenModel();
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
                return Json(new {message = "Номер не валидный", status = 0});

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
            Response.StatusCode = 200; // init status code like always error
  
            if (!ModelState.IsValid)
                return Json(new {message = "данные пользователя не валидны", status = 0});
            

            var findedUser = await _account.Users.GetUserByName(user.phone);
            var findedClient = _account.Clients.GetClientByPhone(user.phone);

            if (findedUser != null || findedClient?.id != null)
                return Json(new { message = "пользователь уже существует в системе", status = 0});
            

            // create client
            var client = _account.Users.ToClient(user);
            var clientCreated=_account.Clients.CreateOrUpdateClient(client);

            if(clientCreated?.id == null)
                return Json(new { message = "ошибка создания пользователя", status = 0 });
            

            // create identity user
            var identityWrapper = await _account.Users.FastUserIdentityCreate(clientCreated);
            if (identityWrapper.IdentityUser != null && identityWrapper.Status != 0)
            {
                Response.StatusCode = 201;
                return Json(new { content = _account.Users.Swap(clientCreated, _crypto.Encrypt), message=identityWrapper.Message,status = identityWrapper.Status });
            }

            
            return Json(new { message = "ошибка создания пользователя", status = 0 });

        }

        [Authorize]
        [HttpPut]
        [PullToken]
        public async Task<IActionResult> EditUser([FromBody] User_DTO user)
        {
            Response.StatusCode = 200;
            var tokenModel = HttpContext.GeTokenModel();
            if (tokenModel != null && user.phone == tokenModel.Phone)
            {          

                if (!ModelState.IsValid){
                    return Json(new { message = "данные пользователя не валидны", status = 0 });
                }


                var findedUser = await _account.Users.GetUserByName(user.phone);
                var findedClient = _account.Clients.GetClientByPhone(user.phone);

                if (findedUser == null || findedClient?.id == null)
                    return Json(new { message = "ошибка редактирования пользователя", status = 0 });

                // update client
                var client = _account.Users.ToClient(user);
                client.barcode = findedClient.barcode;

                var clientCreated = _account.Clients.CreateOrUpdateClient(client);

                if (clientCreated?.id == null)
                    return Json(new { message = "ошибка создания пользователя", status = 0 });

                // update user
                findedUser.Email = clientCreated.email;
                findedUser.NormalizedEmail = clientCreated.email.ToUpper();
                var identityUser = await _account.Users.UserEngine.UpdateAsync(findedUser);

                if (identityUser == null)
                    return Json(new { message = "ошибка создания identity пользователя", status = 0 });

                return Json(new { content = _account.Users.Swap(clientCreated, _crypto.Encrypt), message = "пользователь успешно обновлен", status = 2 });
            }

            Response.StatusCode = 401;
            return Json(new { message = "ошибка авторизации пользователя", status = 0 });
        }

        [Authorize]
        [HttpPost("changePass")]
        [PullToken]
        public async Task<IActionResult> ChangePassword([FromBody] PasswdModel passwd)
        {
            Response.StatusCode = 200;
            var tokenModel = HttpContext.GeTokenModel();
            if (tokenModel != null)
            {
                if (!ModelState.IsValid){
                    return Json(new { message = "данные не валидны", status = 0 });
                }

                var resultCompare = await _account.Users.CheckUser(tokenModel.Phone, passwd.Password);
                if (!resultCompare) return Json(new { message = "не удалось связать пользователя и пароль", status = 0 });

                var identityUser = await _account.Users.GetUserByName(tokenModel.Phone);
                var result = await _account.Users.UserEngine.ChangePasswordAsync(identityUser, passwd.Password, passwd.NewPassword);
                if(!result.Succeeded) return Json(new { message = "ошибка смены пароля", status = 0 });

                return Json(new { message = "пароль успешно изменен", status = 2 });
            }

            Response.StatusCode = 401;
            return Json(new { message = "ошибка авторизации пользователя", status = 0 });
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