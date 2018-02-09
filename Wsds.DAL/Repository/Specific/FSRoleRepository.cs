using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.DAL.Repository.Specific
{
    public class FSRoleRepository:IRoleRepository
    {
        public FSRoleRepository(RoleManager<IdentityRole> roleManager)
        {
            RoleEngine = roleManager;

        }

        public RoleManager<IdentityRole> RoleEngine { get; }

        public async Task<IdentityRole> DeleteRole(string id)
        {
            return await StartAction(id, (role) => RoleEngine.DeleteAsync(role));
        }

        public async Task<IdentityRole> CreateRole(string name)
        {
            var findedRole = await RoleEngine.FindByNameAsync(name);
            if (findedRole != null) return findedRole;

            var newRole = new IdentityRole(name?.ToLower());
            var result = await RoleEngine.CreateAsync(newRole);
            if (!result.Succeeded)
                throw new IdentityNotMappedException(result.Errors.First().Description);

            return newRole;
        }

        public async Task<IdentityRole> EditRole(string id,string name)
        {
            return await StartAction(id, (role) =>
            {
                role.Name = name.ToLower();
                return RoleEngine.UpdateAsync(role);
            });
        }

        public IEnumerable<IdentityRole> AllRoles(Func<IdentityRole, bool> filter)
        {
            return RoleEngine.Roles.AsNoTracking().Where(filter);
        }

        public IdentityRole GetSingleRole(Func<IdentityRole, bool> filter)
        {
            return RoleEngine.Roles.FirstOrDefault(filter);
        }

        #region private helpers
        private async Task<IdentityRole> StartAction(string pointer,
                                                    Func<IdentityRole,Task<IdentityResult>> actionFunc,
                                                    bool isKeySearch=true)
        {
            var findedRole = await ((isKeySearch) ? RoleEngine.FindByIdAsync(pointer) 
                : RoleEngine.FindByNameAsync(pointer));

            if(findedRole==null)
                throw new ObjectNotFoundException("not found role");

            var result = await actionFunc(findedRole);

            if (!result.Succeeded)
                throw new Exception(result.Errors.First()?.Description);

            return findedRole;
        }
        #endregion 
    }
}
