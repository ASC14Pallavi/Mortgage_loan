using Microsoft.AspNetCore.Identity;
using MortgageLoanProcessing.Model;

namespace MortgageLoanProcessing.Data
{
    public class DbInitializer
    {
        public static async Task SeedRolesAndAdminAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            var adminUser = new User
            {
                UserName = "admin@yourdomain.com",
                Email = "admin@yourdomain.com",
                EmailConfirmed = true
            };

            var userExists = await userManager.FindByEmailAsync(adminUser.Email);
            if (userExists == null)
            {
                var createAdmin = await userManager.CreateAsync(adminUser, "Admin@123");
                if (createAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

    }
}

