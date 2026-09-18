using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models.Enums;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels.HomeViewModels;

namespace TaskManager.Services
{
    public class HomeService : IHomeService
    {
        private readonly ApplicationDbContext context;

        public HomeService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<HomeDashboardViewModel> GetDashboardAsync(string userId)
        {
            var accessibleProjects = context.Projects
                .AsNoTracking()
                .Where(p =>
                    p.OwnerId == userId ||
                    p.ProjectMembers.Any(pm => pm.UserId == userId));

            int projectsCount =
                await accessibleProjects.CountAsync();

            int tasksCount =
                await accessibleProjects
                    .SelectMany(p => p.Tasks)
                    .CountAsync();

            int completedTasksCount =
                await accessibleProjects
                    .SelectMany(p => p.Tasks)
                    .CountAsync(t =>
                        t.Status == TaskItemStatus.Completed);

            int completionPercentage = 0;

            if (tasksCount > 0)
            {
                completionPercentage =(int)Math.Round((double)completedTasksCount / tasksCount * 100);
            }

            return new HomeDashboardViewModel
            {
                ProjectsCount = projectsCount,
                TasksCount = tasksCount,
                CompletedTasksCount = completedTasksCount,
                CompletionPercentage = completionPercentage
            };
        }
    }
}