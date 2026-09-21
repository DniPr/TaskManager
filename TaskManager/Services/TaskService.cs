using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Models.Enums;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels.CommentViewModels;
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
                    ProjectId = p.Id,
                    ProjectName = p.Name,
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

                            CanModify = t.Project.OwnerId == userId ||
                                        t.CreatedByUserId == userId
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

                    CommentsCount = t.Comments.Count(),

                    CanModify =
                    t.Project.OwnerId == userId ||
                    t.CreatedByUserId == userId,

                    Comments = t.Comments
                    .OrderBy(c => c.CreatedOn)
                    .Select(c => new CommentViewModel
                    {
                        Id = c.Id,
                        Content = c.Content,
                        CreatedOn = c.CreatedOn,
                        AuthorId = c.AuthorId,
                        AuthorName = c.Author.FirstName + " " + c.Author.LastName
                    })
                    .ToList(),
                    NewComment = new CommentCreateViewModel
                    {
                        TaskItemId = t.Id
                    }
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
                CreatedByUserId = userId,

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
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return false;
            }

            bool canModify =
                task.Project.OwnerId == userId ||
                task.CreatedByUserId == userId;

            if (!canModify)
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
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return false;
            }

            bool canModify =
                task.Project.OwnerId == userId ||
                task.CreatedByUserId == userId;

            if (!canModify)
            {
                return false;
            }

            dbContext.TaskItems.Remove(task);

            await dbContext.SaveChangesAsync();

            return true;
        }
        public async Task<IEnumerable<TaskAssigneeViewModel>> GetProjectAssigneesAsync(int projectId,string userId)
        {
            bool hasAccess = await dbContext.Projects
                .AsNoTracking()
                .AnyAsync(p =>
                    p.Id == projectId &&
                    (p.OwnerId == userId ||
                     p.ProjectMembers.Any(pm => pm.UserId == userId)));

            if (!hasAccess)
            {
                return Enumerable.Empty<TaskAssigneeViewModel>();
            }

            TaskAssigneeViewModel? owner = await dbContext.Projects
                .AsNoTracking()
                .Where(p => p.Id == projectId)
                .Select(p => new TaskAssigneeViewModel
                {
                    Id = p.OwnerId,
                    FullName = p.Owner.FirstName + " " + p.Owner.LastName
                })
                .FirstOrDefaultAsync();

            List<TaskAssigneeViewModel> members = await dbContext.ProjectMembers
                .AsNoTracking()
                .Where(pm => pm.ProjectId == projectId)
                .Select(pm => new TaskAssigneeViewModel
                {
                    Id = pm.UserId,
                    FullName = pm.User.FirstName + " " + pm.User.LastName
                })
                .ToListAsync();

            if (owner != null && !members.Any(m => m.Id == owner.Id))
            {
                members.Add(owner);
            }

            return members
                .OrderBy(m => m.FullName)
                .ToList();
        }
        public async Task<bool> UpdateStatusAsync(int taskId,TaskItemStatus status,string userId)
        {
            TaskItem? task = await dbContext.TaskItems
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
            {
                return false;
            }

            bool canModify =
                task.Project.OwnerId == userId ||
                task.CreatedByUserId == userId;

            if (!canModify)
            {
                return false;
            }

            task.Status = status;

            if (status == TaskItemStatus.Completed)
            {
                task.CompletedOn = DateTime.UtcNow;
            }
            else
            {
                task.CompletedOn = null;
            }

            await dbContext.SaveChangesAsync();

            return true;
        }
    }
}
