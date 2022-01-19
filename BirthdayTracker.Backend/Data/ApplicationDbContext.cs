using BirthdayTracker.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BirthdayTracker.Backend.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuider)
        {
            modelBuider.Entity<IdentityRole>().HasData(new IdentityRole 
            { 
                Id = "0a26e36f-1626-4298-9a97-34a8c4118e08",
                Name = "Admin",
                NormalizedName = "Admin".ToUpper(),
                ConcurrencyStamp = "0a26e36f-1626-4298-9a97-34a8c4118e08" 
            });

            modelBuider.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = "58f3dea3-67eb-4284-b4bd-e4504d8e523e",
                Name = "Owner",
                NormalizedName = "Owner".ToUpper(),
                ConcurrencyStamp = "58f3dea3-67eb-4284-b4bd-e4504d8e523e"
            });
            
            modelBuider.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = "dd62e685-29cf-4dd7-b59b-e44022d88d29",
                Name = "User",
                NormalizedName = "User".ToUpper(),
                ConcurrencyStamp = "dd62e685-29cf-4dd7-b59b-e44022d88d29"
            });

            modelBuider.Entity<IdentityUser>().HasData(new IdentityUser
            {
                Id = "25d733fa-b5ce-41fe-a868-beea7723a3e5",
                UserName = "admin",
                Email = "admin@admin.com",
                EmailConfirmed = true,
                ConcurrencyStamp = "25d733fa-b5ce-41fe-a868-beea7723a3e5",
                NormalizedEmail = "admin@admin.com".ToUpper(),
                NormalizedUserName = "admin".ToUpper(),
                PasswordHash = "AQAAAAEAACcQAAAAENCkLthTPd0r7CXtX5/yOEaaKYTHaxSS19gjdFYysigNskCv/encZ9iMB6heC6TPvA==", // T1VsPaNoCbI@
                SecurityStamp = "25d733fa-b5ce-41fe-a868-beea7723a3e5",
            });

            modelBuider.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string> 
            { 
                RoleId = "0a26e36f-1626-4298-9a97-34a8c4118e08",
                UserId = "25d733fa-b5ce-41fe-a868-beea7723a3e5"
            });

            base.OnModelCreating(modelBuider);
        }

        public DbSet<Employee> Employees { get; set; }
    }
}