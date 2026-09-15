using TaskManager.Areas.Admin.Models;

namespace TaskManager.Areas.Admin.Services.Interfaces
{
    public interface IAdminUserService
    {
        Task<IEnumerable<AdminUserViewModel>> GetAllUsersAsync();
        Task<AdminUserRoleViewModel?> GetUserForRoleEditAsync(string userId);
        Task<bool> ChangeUserRoleAsync(AdminUserRoleViewModel vmodel, string currentAdminId);
    }
}
