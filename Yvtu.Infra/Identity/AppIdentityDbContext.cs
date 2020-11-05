using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Yvtu.Core.Entities;

namespace Yvtu.Infra.Identity
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options) 
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>().ToTable("AppUser");
            builder.Entity<IdentityRole>().ToTable("Role");
            builder.Entity<IdentityUserRole<string>>().ToTable("AppUserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("AppUserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("AppUserLogins");
            builder.Entity<IdentityUserToken<string>>().ToTable("AppUserTokens");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("AppRoleClaims");

            foreach (var entity in builder.Model.GetEntityTypes())
            {
                // Replace table names
                entity.SetTableName(entity.GetTableName().ToSnakeCase());

                // Replace column names            
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.GetColumnName().ToSnakeCase());
                }

                foreach (var key in entity.GetKeys())
                {
                    key.SetName(key.GetName().ToSnakeCase());
                }

                //foreach (var key in entity.GetForeignKeys())
                //{
                //    key.Relational().Name = key.Relational().Name.ToSnakeCase();
                //}

                foreach (var index in entity.GetIndexes())
                {
                    index.SetName(index.GetName().ToSnakeCase());
                }
            }

            builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "admin", NormalizedName = "admin".ToUpper() });
            builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "user", NormalizedName = "user".ToUpper() });
        }


        
    }


    public static class StringExtensions
    {
        public static string ToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input)) { return input; }

            var startUnderscores = Regex.Match(input, @"^_+");
            return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToUpper();
        }
    }
}
