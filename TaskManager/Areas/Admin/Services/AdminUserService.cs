using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.Areas.Admin.Models;
using TaskManager.Areas.Admin.Services.Interfaces;
using TaskManager.Common;
using TaskManager.Models;
using TaskManager.Services.Interfaces;

namespace TaskManager.Areas.Admin.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly UserManager<ApplicationUser> userManager;
        public AdminUserService(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IEnumerable<AdminUserViewModel>> GetAllUsersAsync()
        {
            List<ApplicationUser> users = await userManager.Users
                .AsNoTracking()
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync();

            List<AdminUserViewModel> result = new List<AdminUserViewModel>();

            foreach (ApplicationUser user in users)
            {
                IList<string> roles =
                    await userManager.GetRolesAsync(user);

                result.Add(new AdminUserViewModel
                {
                    Id = user.Id,
                    FullName = user.FirstName + " " + user.LastName,
                    Email = user.Email ?? string.Empty,
                    CreatedOn = user.CreatedOn,
                    Role = roles.FirstOrDefault() ?? "No role"
                });
            }

            return result;
        }


        public async Task<bool> ChangeUserRoleAsync(AdminUserRoleViewModel vmodel,string currentAdminId)
        {
            if (vmodel.Role != RoleConstants.Admin &&
                vmodel.Role != RoleConstants.User)
            {
                return false;
            }

            ApplicationUser? user =
                await userManager.FindByIdAsync(vmodel.UserId);

            if (user == null)
            {
                return false;
            }

            IList<string> currentRoles =
                await userManager.GetRolesAsync(user);

            if (currentRoles.Contains(vmodel.Role))
            {
                return true;
            }

            if (user.Id == currentAdminId &&
                currentRoles.Contains(RoleConstants.Admin) &&
                vmodel.Role == RoleConstants.User)
            {
                IList<ApplicationUser> admins =
                    await userManager.GetUsersInRoleAsync(RoleConstants.Admin);

                if (admins.Count <= 1)
                {
                    return false;
                }
            }

            if (currentRoles.Any())
            {
                IdentityResult removeResult =
                    await userManager.RemoveFromRolesAsync(user,currentRoles);

                if (!removeResult.Succeeded)
                {
                    return false;
                }
            }

            IdentityResult addResult =
                await userManager.AddToRoleAsync(user,vmodel.Role);

            return addResult.Succeeded;
        }

        public async Task<AdminUserRoleViewModel?> GetUserForRoleEditAsync(string userId)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            IList<string> roles =
                await userManager.GetRolesAsync(user);

            return new AdminUserRoleViewModel
            {
                UserId = user.Id,
                FullName = user.FirstName + " " + user.LastName,
                Email = user.Email ?? string.Empty,
                Role = roles.FirstOrDefault() ?? RoleConstants.User
            };
        }
    }
}
