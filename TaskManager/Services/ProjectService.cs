using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Models.Enums;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels.ProjectViewModels;

namespace TaskManager.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext dbContext;
        public ProjectService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<ProjectIndexViewModel>> GetAllAsync(string userId)
        {
            return await dbContext.Projects
                .AsNoTracking()
                .Where(p =>
                    p.OwnerId == userId ||
                    p.ProjectMembers.Any(pm => pm.UserId == userId))
                .Select(p => new ProjectIndexViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedOn = p.CreatedOn,
                    Deadline = p.Deadline,
                    OwnerName = p.Owner.FirstName + " " + p.Owner.LastName,
                    MembersCount = p.ProjectMembers.Count(),
                    TasksCount = p.Tasks.Count(),
                    CompletedTasksCount = p.Tasks.Count(t =>
                        t.Status == TaskItemStatus.Completed)
                })
                .OrderByDescending(p => p.CreatedOn)
                .ToListAsync();
        }

        public async Task<ProjectDetailsViewModel?> GetDetailsAsync(int id, string userId)
        {
            return await dbContext.Projects
                .AsNoTracking()
                .Where(p =>
                    p.Id == id &&
                    (p.OwnerId == userId ||
                     p.ProjectMembers.Any(pm => pm.UserId == userId)))
                .Select(p => new ProjectDetailsViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedOn = p.CreatedOn,
                    Deadline = p.Deadline,
                    OwnerId = p.OwnerId,
                    OwnerName = p.Owner.FirstName + " " + p.Owner.LastName,
                    MembersCount = p.ProjectMembers.Count(),
                    TasksCount = p.Tasks.Count(),
                    CompletedTasksCount = p.Tasks.Count(t =>
                        t.Status == TaskItemStatus.Completed),
                    IsOwner = p.OwnerId == userId
                })
                .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(ProjectCreateViewModel vmodel, string ownerId)
        {
            Project project = new Project
            {
                Name = vmodel.Name,
                Description = vmodel.Description,
                Deadline = vmodel.Deadline,
                OwnerId = ownerId,
                CreatedOn = DateTime.UtcNow
            };

            await dbContext.Projects.AddAsync(project);
            await dbContext.SaveChangesAsync();
        }

        public async Task<ProjectEditViewModel?> GetForEditAsync(int id, string userId)
        {
            return await dbContext.Projects
                .AsNoTracking()
                .Where(p => p.Id == id && p.OwnerId == userId)
                .Select(p => new ProjectEditViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Deadline = p.Deadline
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> EditAsync(int id, ProjectEditViewModel vmodel, string userId)
        {
            Project? project = await dbContext.Projects
                .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == userId);

            if (project == null)
            {
                return false;
            }

            project.Name = vmodel.Name;
            project.Description = vmodel.Description;
            project.Deadline = vmodel.Deadline;

            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id, string userId)
        {
            Project? project = await dbContext.Projects
                .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == userId);

            if (project == null)
            {
                return false;
            }

            dbContext.Projects.Remove(project);
            await dbContext.SaveChangesAsync();

            return true;
        }
    }
}
