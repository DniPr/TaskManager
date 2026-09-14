using Microsoft.AspNetCore.Identity;
using TaskManager.Common;

namespace TaskManager.Data.Seed
{
    public class RoleSeed
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(RoleConstants.Admin))
            {
                await roleManager.CreateAsync(
                    new IdentityRole(RoleConstants.Admin));
            }

            if (!await roleManager.RoleExistsAsync(RoleConstants.User))
            {
                await roleManager.CreateAsync(
                    new IdentityRole(RoleConstants.User));
            }
        }
    }
}
