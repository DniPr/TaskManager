using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels.CommentViewModels;

namespace TaskManager.Services
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext dbContext;
        public CommentService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        public async Task<bool> CreateAsync(CommentCreateViewModel vmodel,string userId)
        {
            bool hasAccess = await dbContext.TaskItems
                .AsNoTracking()
                .AnyAsync(t =>
                    t.Id == vmodel.TaskItemId &&
                    (t.Project.OwnerId == userId ||
                     t.Project.ProjectMembers.Any(pm => pm.UserId == userId)));

            if (!hasAccess)
            {
                return false;
            }

            Comment comment = new Comment
            {
                Content = vmodel.Content,
                TaskItemId = vmodel.TaskItemId,
                AuthorId = userId,
                CreatedOn = DateTime.UtcNow
            };

            await dbContext.Comments.AddAsync(comment);
            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id,string userId)
        {
            Comment? comment = await dbContext.Comments
                .FirstOrDefaultAsync(c =>
                    c.Id == id &&
                    c.AuthorId == userId);

            if (comment == null)
            {
                return false;
            }

            dbContext.Comments.Remove(comment);
            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<CommentViewModel>?> GetAllByTaskAsync(int taskId,string userId)
        {
            bool hasAccess = await dbContext.TaskItems
                .AsNoTracking()
                .AnyAsync(t =>
                    t.Id == taskId &&
                    (t.Project.OwnerId == userId ||
                     t.Project.ProjectMembers.Any(pm => pm.UserId == userId)));

            if (!hasAccess)
            {
                return null;
            }

            return await dbContext.Comments
                .AsNoTracking()
                .Where(c => c.TaskItemId == taskId)
                .Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedOn = c.CreatedOn,
                    AuthorId = c.AuthorId,
                    AuthorName = c.Author.FirstName + " " + c.Author.LastName
                })
                .OrderBy(c => c.CreatedOn)
                .ToListAsync();
        }
    }
}
