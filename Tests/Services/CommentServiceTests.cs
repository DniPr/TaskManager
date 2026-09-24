using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Services;
using TaskManager.ViewModels.CommentViewModels;

namespace TaskManager.Tests.Services
{
    [TestFixture]
    public class CommentServiceTests
    {
        private ApplicationDbContext context = null!;
        private CommentService commentService = null!;

        [SetUp]
        public void SetUp()
        {
            DbContextOptions<ApplicationDbContext> options =new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;

            context = new ApplicationDbContext(options);

            commentService = new CommentService(context);
        }

        [TearDown]
        public void TearDown()
        {
            context.Dispose();
        }

        [Test]
        public async Task CreateAsync_ShouldCreateComment_WhenUserHasProjectAccess()
        {
            string ownerId = "owner-id";

            Project project = new Project
            {
                Id = 1,
                Name = "Test Project",
                OwnerId = ownerId,
                CreatedOn = DateTime.UtcNow
            };

            TaskItem task = new TaskItem
            {
                Id = 1,
                Title = "Test Task",
                ProjectId = project.Id,
                Project = project,
                CreatedByUserId = ownerId
            };

            await context.Projects.AddAsync(project);
            await context.TaskItems.AddAsync(task);
            await context.SaveChangesAsync();

            CommentCreateViewModel vmodel =
                new CommentCreateViewModel
                {
                    TaskItemId = task.Id,
                    Content = "Test comment"
                };

            bool result = await commentService.CreateAsync(vmodel,ownerId);

            Assert.That(result, Is.True);

            Comment? comment =
                await context.Comments
                    .FirstOrDefaultAsync();

            Assert.That(comment, Is.Not.Null);
            Assert.That(comment!.Content, Is.EqualTo("Test comment"));
            Assert.That(comment.AuthorId, Is.EqualTo(ownerId));
            Assert.That(comment.TaskItemId, Is.EqualTo(task.Id));
        }

        [Test]
        public async Task DeleteAsync_ShouldDeleteComment_WhenUserIsAuthor()
        {
            string authorId = "author-id";

            Comment comment = new Comment
            {
                Id = 1,
                Content = "Test comment",
                AuthorId = authorId,
                TaskItemId = 1,
                CreatedOn = DateTime.UtcNow
            };

            await context.Comments.AddAsync(comment);
            await context.SaveChangesAsync();

            bool result = await commentService.DeleteAsync(comment.Id,authorId);

            Assert.That(result, Is.True);

            Comment? deletedComment =
                await context.Comments.FindAsync(comment.Id);

            Assert.That(deletedComment, Is.Null);
        }

        [Test]
        public async Task DeleteAsync_ShouldReturnFalse_WhenUserIsNotAuthor()
        {
            string authorId = "author-id";
            string otherUserId = "other-user-id";

            Comment comment = new Comment
            {
                Id = 1,
                Content = "Test comment",
                AuthorId = authorId,
                TaskItemId = 1,
                CreatedOn = DateTime.UtcNow
            };

            await context.Comments.AddAsync(comment);
            await context.SaveChangesAsync();

            bool result = await commentService.DeleteAsync(comment.Id,otherUserId);

            Assert.That(result, Is.False);

            Comment? existingComment =
                await context.Comments.FindAsync(comment.Id);

            Assert.That(existingComment, Is.Not.Null);
        }
    }
}