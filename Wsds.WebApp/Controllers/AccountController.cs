using System.Linq;
using System.Text.RegularExpressions;
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
                    //TODO: check this role method when we will be create admin panel
                    //var roles = await _userRepository.UserEngine.GetRolesAsync(user);

                    // get clients
                    var client = _account.Clients.GetClientByPhone(user.UserName).FirstOrDefault();
                    if (client != null)
                    {
                        var jToken = AuthOpt.GetToken(user,client.id);
                        // get favorite stores
                        //var favoriteStores = _account.Clients.GetFavoriteStore(user.Card);
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
                var client = _account.Clients.GetClientByPhone(tokenModel.Phone).FirstOrDefault();
                if (client != null)
                {
                    //var favoriteStores = _account.Clients.GetFavoriteStore(tokenModel.Client);
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
            var client = _account.Clients.GetClientByPhone(vm.Phone).FirstOrDefault();
            var data = await _account.Users.UserVerifyStrategy(vm.Phone, user, client);

            // send json
            return Json(new {message = data.Item1, status = data.Item2});
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] User_DTO user)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
                return Json(new {message = "данные пользователя не валидны", status = 0});
            }

            var findedUser = await _account.Users.GetUserByName(user.phone);
            var findedClient = _account.Clients.GetClientByPhone(user.phone);

            if (findedUser != null || findedClient != null)
            {
                Response.StatusCode = 400;
                return Json(new {message = "пользователь уже существует в системе", status = 0});
            }

            //TODO:logic create with typhoon sync
            return null;
        }
    }
}