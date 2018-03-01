using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using wsdsCoreApi.Filters;
using Wsds.DAL.Identity;
using Wsds.DAL.Repository.Abstract;
using Wsds.WebApp.Models;

namespace Wsds.WebApp.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UserController(IUserRepository userRepository,IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        [HttpGet]
        public IActionResult AllUsers()
        {
            return Ok(_userRepository.Users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userRepository.GetUserById(id);
            return Ok(user);
        }

        [HttpGet("login/{login}")]
        public async Task<IActionResult> GetUserByUserName(string login)
        {
            var user = await _userRepository.GetUserByName(login);
            return Ok(user);
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userRepository.GetUserByEmail(email);
            return Ok(user);
        }

        [HttpGet("roles/{id}")]
        public IActionResult GetRolesByUser(string id)
        {
            if (String.IsNullOrEmpty(id))
                return BadRequest("id is empty");

            var roles = _userRepository.UserRoles(id)
                .Join(_roleRepository.AllRoles(p => true), p => p, c => c.Name, (p, c) => new RoleShortModel()
                {
                    Id = c.Id,
                    Name = c.Name
                });
            return Ok(roles);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(RegisterModel auth)
        {
            if (!ModelState.IsValid)
                return BadRequest( "Model is not valid");

            var appUser = new AppUser() { UserName = auth.Phone.ToLower(), Email = auth.Email.ToLower()};

            var usr = await _userRepository.CreateUser(appUser, auth.Password);
            return Ok(usr.Id);

        }

        [HttpPost("editroles")]
        public async Task<IActionResult> EditRolesForUser(string id,string[] roles)
        {
            var user = await _userRepository.GetUserById(id);
            if (user == null)
                return BadRequest("user can not be found");

            var userRoles = _userRepository.UserRoles(id);
            var enumerable = userRoles as string[] ?? userRoles.ToArray();
            var newRoles = roles.Except(enumerable);
            var oldRoles = enumerable.Except(roles);

            await _userRepository.UserEngine.AddToRolesAsync(user, newRoles);
            await _userRepository.UserEngine.RemoveFromRolesAsync(user,oldRoles);

            return Ok(newRoles);
        }

        [HttpDelete("{id:maxlength(36)}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (String.IsNullOrEmpty(id))
                return BadRequest("current id is not valid");

            var finded = await _userRepository.DeleteUser(id);
            return Ok(finded.Id);
        }

        [HttpPut]
        [FieldValidIgnore("Password")]
        public async Task<IActionResult> UpdateUser(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var usr = await _userRepository.UpdateUser(new AppUser()
                {
                    Id = model.Id,
                    UserName = model.Phone,
                    Email = model.Email
                });

                return Ok(usr.Id);
            }

            return BadRequest("current model of new user is not valid");
        }
    }
}