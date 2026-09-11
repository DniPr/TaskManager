using TaskManager.ViewModels.ProjectMemberViewModels;

namespace TaskManager.Services.Interfaces
{
    public interface IProjectMemberService
    {
        Task<ProjectMembersPageViewModel?> GetMembersAsync(int projectId,string userId);
        Task<bool> AddMemberAsync(ProjectMemberAddViewModel vmodel,string userId);

        Task<bool> RemoveMemberAsync(int projectId,string memberUserId,string userId);
    }
}
