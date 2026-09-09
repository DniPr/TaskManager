using TaskManager.ViewModels.TaskViewModels;

namespace TaskManager.Services.Interfaces
{
    public interface ITaskService
    {
        Task<TaskIndexPageViewModel?> GetAllByProjectAsync(int projectId,string userId);

        Task<TaskDetailsViewModel?> GetDetailsAsync(int id,string userId);

        Task CreateAsync(TaskCreateViewModel vmodel,string userId);

        Task<TaskEditViewModel?> GetForEditAsync(int id,string userId);

        Task<bool> EditAsync(int id,TaskEditViewModel vmodel,string userId);

        Task<bool> DeleteAsync(int id,string userId);
    }
}
