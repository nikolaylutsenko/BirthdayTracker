using BirthdayTracker.Shared;
using BirthdayTracker.Shared.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BirthdayTracker.Backend.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, string,
        IdentityUserClaim<string>, AppUserAppRole, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuider)
        {
            base.OnModelCreating(modelBuider);

            modelBuider.Entity<AppUser>(b =>
            {
                // Each User can have many UserClaims
                b.HasMany(e => e.Claims)
                    .WithOne()
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired();

                // Each User can have many UserLogins
                b.HasMany(e => e.Logins)
                    .WithOne()
                    .HasForeignKey(ul => ul.UserId)
                    .IsRequired();

                // Each User can have many UserTokens
                b.HasMany(e => e.Tokens)
                    .WithOne()
                    .HasForeignKey(ut => ut.UserId)
                    .IsRequired();

                // Each User can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            modelBuider.Entity<AppRole>(b =>
            {
                // Each Role can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();
            });

            modelBuider.Entity<AppRole>().HasData(new AppRole 
            { 
                Id = "0a26e36f-1626-4298-9a97-34a8c4118e08",
                Name = "Admin",
                NormalizedName = "Admin".ToUpper(),
                ConcurrencyStamp = "0a26e36f-1626-4298-9a97-34a8c4118e08" 
            });

            modelBuider.Entity<AppRole>().HasData(new AppRole
            {
                Id = "58f3dea3-67eb-4284-b4bd-e4504d8e523e",
                Name = "Owner",
                NormalizedName = "Owner".ToUpper(),
                ConcurrencyStamp = "58f3dea3-67eb-4284-b4bd-e4504d8e523e"
            });
            
            modelBuider.Entity<AppRole>().HasData(new AppRole
            {
                Id = "dd62e685-29cf-4dd7-b59b-e44022d88d29",
                Name = "User",
                NormalizedName = "User".ToUpper(),
                ConcurrencyStamp = "dd62e685-29cf-4dd7-b59b-e44022d88d29"
            });

            modelBuider.Entity<AppUser>().HasData(new AppUser
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
                BirthDay = DateTime.Parse("2022-01-22T00:00:00"),
                Name = "Admin",
                Surname = "Admin",
                PositionName = "Admin",
            });

            modelBuider.Entity<AppUser>().ToTable("AppUser");

            modelBuider.Entity<AppRole>().ToTable("AppRole");

            modelBuider.Entity<AppUserAppRole>().ToTable("AppUserAppRole");

            modelBuider.Entity<AppUserAppRole>().HasData(new AppUserAppRole
            { 
                RoleId = "0a26e36f-1626-4298-9a97-34a8c4118e08",
                UserId = "25d733fa-b5ce-41fe-a868-beea7723a3e5"
            });

            modelBuider.Entity<Company>()
                .HasMany(emp => emp.Employees)
                .WithOne(company => company.Company);
        }

        public DbSet<Company> Companies { get;set;}
    }
}