﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleRecaptcha.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GoogleRecaptcha.Data
{
    public class AuthDBContext : IdentityDbContext<SystemUser>
    {
        public AuthDBContext(DbContextOptions<AuthDBContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
