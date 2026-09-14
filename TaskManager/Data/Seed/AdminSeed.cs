using Microsoft.AspNetCore.Identity;
using TaskManager.Common;
using TaskManager.Models;

namespace TaskManager.Data.Seed
{
    public static class AdminSeed
    {
        public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager,IConfiguration configuration)
        {
            string? adminEmail =
                configuration["AdminUser:Email"];

            string? adminPassword =
                configuration["AdminUser:Password"];

            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
            {
                return;
            }

            ApplicationUser? admin =
                await userManager.FindByEmailAsync(adminEmail);

            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    FirstName = "System",
                    LastName = "Administrator",
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    CreatedOn = DateTime.UtcNow
                };

                IdentityResult result =
                    await userManager.CreateAsync(
                        admin,
                        adminPassword);

                if (!result.Succeeded)
                {
                    return;
                }
            }

            if (!await userManager.IsInRoleAsync(admin,RoleConstants.Admin))
            {
                await userManager.AddToRoleAsync(admin,RoleConstants.Admin);
            }
        }
    }
}