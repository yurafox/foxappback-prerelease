using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Wsds.DAL.Repository.Abstract;
using Wsds.WebApp.Auth;
using Wsds.WebApp.Models;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public AccountController(IUserRepository userRepository,IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        [HttpPost("token")]
        public async Task Token(AuthModel auth)
        {
            var resultMessage = "Invalid Authentication Data";
            if (ModelState.IsValid)
            {
                var resultCompare = await _userRepository.CheckUser(auth.Login, auth.Password);
                if (!resultCompare) resultMessage = "Login or Password incorrect";
                else
                {
                    var user = await _userRepository.UserEngine.FindByNameAsync(auth.Login.ToLower());
                    //TODO: check this role method when we will be create admin panel
                    //var roles = await _userRepository.UserEngine.GetRolesAsync(user);

                    var jToken = AuthOpt.GetToken(user);

                    var responseObj = new
                    {
                        access_token = jToken,
                        user_guid = user.Id
                    };

                    await Response.WriteAsync(JsonConvert.SerializeObject(responseObj, new JsonSerializerSettings { Formatting = Formatting.Indented }));
                    return;
                }
            }

            Response.StatusCode = 400;
            await Response.WriteAsync(resultMessage);
        }

    }
}