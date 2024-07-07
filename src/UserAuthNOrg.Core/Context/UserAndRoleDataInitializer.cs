using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using UserAuthNOrg.Core.Models;
using UserAuthNOrg.Utilities.CoreConstants;

namespace UserAuthNOrg.Core.Context
{
    public class UserAndRoleDataInitializer
    {
        public static void SeedData(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        private static void SeedUsers(UserManager<User> userManager)
        {
            if (userManager.FindByEmailAsync("admin@lms.com").Result == null)
            {
                var password = "Admin_4Lm";
               
                var user = new User()
                {
                    FirstName = "Admin",
                    LastName = "Lms",
                    UserName = "admin@lms.com",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Email = "admin@lms.com",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    PhoneNumber = "08061517674",
                    PhoneNumberConfirmed = true
                };

                var result = userManager.CreateAsync(user, password).Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, ConstantsString.SuperAdmin).Wait();
                }
            }

        }

        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync(ConstantsString.SuperAdmin).Result)
            {
                var role = new IdentityRole
                {
                    Name = ConstantsString.SuperAdmin
                };
                var roleResult = roleManager.CreateAsync(role).Result;
            }


            if (!roleManager.RoleExistsAsync(ConstantsString.OrgUser).Result)
            {
                var role = new IdentityRole
                {
                    Name = ConstantsString.OrgUser
                };
                var roleResult = roleManager.CreateAsync(role).Result;
            }
        }
    }
}
