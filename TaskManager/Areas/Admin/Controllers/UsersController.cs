using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.Areas.Admin.Models;
using TaskManager.Areas.Admin.Services.Interfaces;
using TaskManager.Common;
using TaskManager.Services.Interfaces;

namespace TaskManager.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleConstants.Admin)]
    public class UsersController : Controller
    {
        private readonly IAdminUserService adminUserService;
        public UsersController(IAdminUserService adminUserService)
        {
            this.adminUserService = adminUserService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await adminUserService.GetAllUsersAsync();

            return View(users);
        }


        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            AdminUserRoleViewModel? vmodel =
                await adminUserService.GetUserForRoleEditAsync(id);

            if (vmodel == null)
            {
                return NotFound();
            }

            return View(vmodel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(AdminUserRoleViewModel vmodel)
        {
            if (!ModelState.IsValid)
            {
                AdminUserRoleViewModel? loadedVmodel =
                    await adminUserService.GetUserForRoleEditAsync(vmodel.UserId);

                if (loadedVmodel == null)
                {
                    return NotFound();
                }

                loadedVmodel.Role = vmodel.Role;

                return View(loadedVmodel);
            }

            string currentAdminId =
                User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            bool isChanged =
                await adminUserService.ChangeUserRoleAsync(vmodel,currentAdminId);

            if (!isChanged)
            {
                AdminUserRoleViewModel? loadedVmodel =
                    await adminUserService.GetUserForRoleEditAsync(vmodel.UserId);

                if (loadedVmodel == null)
                {
                    return NotFound();
                }

                ModelState.AddModelError(
                    string.Empty,
                    "The role could not be changed.");

                return View(loadedVmodel);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}