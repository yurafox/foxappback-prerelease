using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Identity;
using Wsds.DAL.Repository.Abstract;
using Wsds.WebApp.Models;

namespace Wsds.WebApp.Controllers
{
    [Authorize(Roles = "admin")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class RoleController : Controller
    {
        private readonly IRoleRepository _roleRepository;

        public RoleController(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        [HttpGet]
        public IActionResult AllRoles()
        {
            return Ok(_roleRepository.AllRoles((role)=> true));
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {

            if (String.IsNullOrEmpty(roleName))
                return BadRequest("role name is not valid");

            var role = await _roleRepository.CreateRole(roleName);
            return Ok(new { id=role.Id,name=role.Name});

        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRole(string id)
        {
            if (String.IsNullOrEmpty(id))
                return BadRequest("current id is not valid");

            var finded = await _roleRepository.DeleteRole(id);
            return Ok(new {id=finded.Id,name=finded.Name});
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRole(RoleShortModel role)
        {

            if (!ModelState.IsValid)
                return BadRequest("current model is not valid");

            var finded = await _roleRepository.EditRole(role.Id, role.Name);
            return Ok(new { id = finded.Id, name = finded.Name });
        }
    }
}