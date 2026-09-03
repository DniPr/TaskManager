using TaskManager.ViewModels.ProjectViewModels;

namespace TaskManager.Services.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectIndexViewModel>> GetAllAsync(string userId);

        Task<ProjectDetailsViewModel?> GetDetailsAsync(int id, string userId);

        Task CreateAsync(ProjectCreateViewModel vmodel, string ownerId);

        Task<ProjectEditViewModel?> GetForEditAsync(int id, string userId);

        Task<bool> EditAsync(int id, ProjectEditViewModel vmodel, string userId);

        Task<bool> DeleteAsync(int id, string userId);
    }
}
