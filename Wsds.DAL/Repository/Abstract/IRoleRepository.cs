using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Wsds.DAL.Repository.Abstract
{
    public interface IRoleRepository
    {
        Task<IdentityRole> DeleteRole(string id);
        Task<IdentityRole> CreateRole(string name);
        Task<IdentityRole> EditRole(string role, string name); 
        IEnumerable<IdentityRole> AllRoles(Func<IdentityRole, bool> filter);
        IdentityRole GetSingleRole(Func<IdentityRole, bool> filter);
    }
}
