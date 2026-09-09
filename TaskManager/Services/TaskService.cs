using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Models.Enums;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels.TaskViewModels;

namespace TaskManager.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext dbContext;
        public TaskService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<TaskIndexPageViewModel?> GetAllByProjectAsync(int projectId,string userId)
        {
            TaskIndexPageViewModel? result = await dbContext.Projects
                .AsNoTracking()
                .Where(p =>
                    p.Id == projectId &&
                    (p.OwnerId == userId ||
                     p.ProjectMembers.Any(pm => pm.UserId == userId)))
                .Select(p => new TaskIndexPageViewModel
                {
                    Tasks = p.Tasks
                        .Select(t => new TaskIndexViewModel
                        {
                            Id = t.Id,
                            Title = t.Title,
                            Status = t.Status,
                            Priority = t.Priority,
                            Deadline = t.Deadline,

                            AssignedUserName = t.AssignedUser == null
                                ? null
                                : t.AssignedUser.FirstName
                                  + " "
                                  + t.AssignedUser.LastName,

                            ProjectId = t.ProjectId,
                            ProjectName = p.Name
                        })
                        .OrderBy(t => t.Status)
                        .ThenByDescending(t => t.Priority)
                        .ThenBy(t => t.Deadline)
                        .ToList()
                })
                .FirstOrDefaultAsync();

            return result;
        }
        public async Task<TaskDetailsViewModel?> GetDetailsAsync(int id,string userId)
        {
            return await dbContext.TaskItems
                .AsNoTracking()
                .Where(t =>
                    t.Id == id &&
                    (t.Project.OwnerId == userId ||
                     t.Project.ProjectMembers.Any(pm => pm.UserId == userId)))
                .Select(t => new TaskDetailsViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    Priority = t.Priority,
                    CreatedOn = t.CreatedOn,
                    Deadline = t.Deadline,
                    CompletedOn = t.CompletedOn,

                    ProjectId = t.ProjectId,
                    ProjectName = t.Project.Name,

                    AssignedUserId = t.AssignedUserId,

                    AssignedUserName = t.AssignedUser == null
                        ? null
                        : t.AssignedUser.FirstName + " " + t.AssignedUser.LastName,

                    CommentsCount = t.Comments.Count()
                })
                .FirstOrDefaultAsync();
        }
        public async Task CreateAsync(TaskCreateViewModel vmodel,string userId)
        {
            bool hasAccess = await dbContext.Projects
                .AsNoTracking()
                .AnyAsync(p =>
                    p.Id == vmodel.ProjectId &&
                    (p.OwnerId == userId ||
                     p.ProjectMembers.Any(pm => pm.UserId == userId)));

            if (!hasAccess)
            {
                throw new UnauthorizedAccessException();
            }

            if (!string.IsNullOrWhiteSpace(vmodel.AssignedUserId))
            {
                bool isValidAssignedUser = await dbContext.Projects
                    .AsNoTracking()
                    .AnyAsync(p =>
                        p.Id == vmodel.ProjectId &&
                        (p.OwnerId == vmodel.AssignedUserId ||
                         p.ProjectMembers.Any(pm =>
                             pm.UserId == vmodel.AssignedUserId)));

                if (!isValidAssignedUser)
                {
                    throw new ArgumentException("Assigned user is not a member of the project.");
                }
            }

            TaskItem task = new TaskItem
            {
                Title = vmodel.Title,
                Description = vmodel.Description,
                Priority = vmodel.Priority,
                Deadline = vmodel.Deadline,
                ProjectId = vmodel.ProjectId,
                AssignedUserId = vmodel.AssignedUserId,

                Status = TaskItemStatus.ToDo,
                CreatedOn = DateTime.UtcNow
            };

            await dbContext.TaskItems.AddAsync(task);
            await dbContext.SaveChangesAsync();
        }
        public async Task<TaskEditViewModel?> GetForEditAsync(int id,string userId)
        {
            return await dbContext.TaskItems
                .AsNoTracking()
                .Where(t =>
                    t.Id == id &&
                    (t.Project.OwnerId == userId ||
                     t.Project.ProjectMembers.Any(pm => pm.UserId == userId)))
                .Select(t => new TaskEditViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    Priority = t.Priority,
                    Deadline = t.Deadline,
                    ProjectId = t.ProjectId,
                    AssignedUserId = t.AssignedUserId
                })
                .FirstOrDefaultAsync();
        }
        public async Task<bool> EditAsync(int id,TaskEditViewModel vmodel,string userId)
        {
            TaskItem? task = await dbContext.TaskItems
                .Include(t => t.Project)
                    .ThenInclude(p => p.ProjectMembers)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return false;
            }

            bool hasAccess =
                task.Project.OwnerId == userId ||
                task.Project.ProjectMembers.Any(pm => pm.UserId == userId);

            if (!hasAccess)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(vmodel.AssignedUserId))
            {
                bool isValidAssignedUser =
                    task.Project.OwnerId == vmodel.AssignedUserId ||
                    task.Project.ProjectMembers.Any(pm =>
                        pm.UserId == vmodel.AssignedUserId);

                if (!isValidAssignedUser)
                {
                    return false;
                }
            }

            task.Title = vmodel.Title;
            task.Description = vmodel.Description;
            task.Priority = vmodel.Priority;
            task.Deadline = vmodel.Deadline;
            task.AssignedUserId = vmodel.AssignedUserId;

            if (task.Status != TaskItemStatus.Completed &&
                vmodel.Status == TaskItemStatus.Completed)
            {
                task.CompletedOn = DateTime.UtcNow;
            }
            else if (task.Status == TaskItemStatus.Completed &&
                     vmodel.Status != TaskItemStatus.Completed)
            {
                task.CompletedOn = null;
            }

            task.Status = vmodel.Status;

            await dbContext.SaveChangesAsync();

            return true;
        }
        public async Task<bool> DeleteAsync(int id,string userId)
        {
            TaskItem? task = await dbContext.TaskItems
                .Include(t => t.Project)
                    .ThenInclude(p => p.ProjectMembers)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return false;
            }

            bool hasAccess =
                task.Project.OwnerId == userId ||
                task.Project.ProjectMembers.Any(pm => pm.UserId == userId);

            if (!hasAccess)
            {
                return false;
            }

            dbContext.TaskItems.Remove(task);

            await dbContext.SaveChangesAsync();

            return true;
        }
    }
}
