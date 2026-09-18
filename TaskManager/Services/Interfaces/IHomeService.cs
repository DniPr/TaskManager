using TaskManager.ViewModels.HomeViewModels;

namespace TaskManager.Services.Interfaces
{
    public interface IHomeService
    {
        Task<HomeDashboardViewModel> GetDashboardAsync(string userId);
    }
}