﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace Wsds.DAL.Identity
{
    public class AppUser : IdentityUser
    {
        public long Card { get; set; }
    }
}
