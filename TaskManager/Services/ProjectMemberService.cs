using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels.ProjectMemberViewModels;

namespace TaskManager.Services
{
    public class ProjectMemberService : IProjectMemberService
    {
        private readonly ApplicationDbContext dbContext;
        public ProjectMemberService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> AddMemberAsync(ProjectMemberAddViewModel vmodel,string userId)
        {
            var project = await dbContext.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(p =>
                    p.Id == vmodel.ProjectId &&
                    p.OwnerId == userId);

            if (project == null)
            {
                return false;
            }

            ApplicationUser? userToAdd = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == vmodel.Email);

            if (userToAdd == null)
            {
                return false;
            }

            if (userToAdd.Id == project.OwnerId)
            {
                return false;
            }

            bool alreadyMember = await dbContext.ProjectMembers
                .AnyAsync(pm =>
                    pm.ProjectId == vmodel.ProjectId &&
                    pm.UserId == userToAdd.Id);

            if (alreadyMember)
            {
                return false;
            }

            ProjectMember projectMember = new ProjectMember
            {
                ProjectId = vmodel.ProjectId,
                UserId = userToAdd.Id,
                JoinedOn = DateTime.UtcNow
            };

            await dbContext.ProjectMembers.AddAsync(projectMember);
            await dbContext.SaveChangesAsync();

            return true;
        }
        public async Task<ProjectMembersPageViewModel?> GetMembersAsync(int projectId,string userId)
        {
            return await dbContext.Projects
                .AsNoTracking()
                .Where(p =>
                    p.Id == projectId &&
                    (p.OwnerId == userId ||
                     p.ProjectMembers.Any(pm => pm.UserId == userId)))
                .Select(p => new ProjectMembersPageViewModel
                {
                    ProjectId = p.Id,
                    ProjectName = p.Name,
                    IsOwner = p.OwnerId == userId,

                    Members = p.ProjectMembers
                        .Select(pm => new ProjectMemberViewModel
                        {
                            UserId = pm.UserId,
                            FullName = pm.User.FirstName + " " + pm.User.LastName,
                            Email = pm.User.Email!,
                            JoinedOn = pm.JoinedOn
                        })
                        .OrderBy(pm => pm.FullName)
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }
        public async Task<bool> RemoveMemberAsync(int projectId,string memberUserId,string userId)
        {
            bool isOwner = await dbContext.Projects
                .AsNoTracking()
                .AnyAsync(p =>
                    p.Id == projectId &&
                    p.OwnerId == userId);

            if (!isOwner)
            {
                return false;
            }

            ProjectMember? projectMember = await dbContext.ProjectMembers
                .FirstOrDefaultAsync(pm =>
                    pm.ProjectId == projectId &&
                    pm.UserId == memberUserId);

            if (projectMember == null)
            {
                return false;
            }

            dbContext.ProjectMembers.Remove(projectMember);
            await dbContext.SaveChangesAsync();

            return true;
        }
    }
}
