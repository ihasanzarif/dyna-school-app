using DynaSchoolApp.Models.Entities;
using DynaSchoolApp.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynaSchoolApp.Database.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<ProductModel> Products {  get; set; }
        public DbSet<RoleModel> Roles { get; set; }
        public DbSet<SchoolUserModel> SchoolUsers { get; set; }
        public DbSet<SchoolUserRoleModel> SchoolUserRoles { get; set; }
        public DbSet<RefreshTokenModel> RefreshTokens { get; set; }
    }
}
