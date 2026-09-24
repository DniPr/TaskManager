using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Models.Enums;
using TaskManager.Services;

namespace TaskManager.Tests.Services
{
    [TestFixture]
    public class HomeServiceTests
    {
        private ApplicationDbContext context = null!;
        private HomeService homeService = null!;

        [SetUp]
        public void SetUp()
        {
            DbContextOptions<ApplicationDbContext> options =new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;

            context = new ApplicationDbContext(options);

            homeService = new HomeService(context);
        }

        [TearDown]
        public void TearDown()
        {
            context.Dispose();
        }

        [Test]
        public async Task GetDashboardAsync_ShouldReturnCorrectCounts_ForProjectOwner()
        {
            string userId = "owner-id";

            Project project = new Project
            {
                Id = 1,
                Name = "Test Project",
                OwnerId = userId,
                CreatedOn = DateTime.UtcNow
            };

            TaskItem firstTask = new TaskItem
            {
                Id = 1,
                Title = "Task 1",
                ProjectId = project.Id,
                CreatedByUserId = userId,
                Status = TaskItemStatus.Completed
            };

            TaskItem secondTask = new TaskItem
            {
                Id = 2,
                Title = "Task 2",
                ProjectId = project.Id,
                CreatedByUserId = userId,
                Status = TaskItemStatus.InProgress
            };

            await context.Projects.AddAsync(project);
            await context.TaskItems.AddRangeAsync(firstTask,secondTask);

            await context.SaveChangesAsync();

            var result =
                await homeService.GetDashboardAsync(userId);

            Assert.That(result.ProjectsCount, Is.EqualTo(1));
            Assert.That(result.TasksCount, Is.EqualTo(2));
            Assert.That(result.CompletedTasksCount, Is.EqualTo(1));
            Assert.That(result.CompletionPercentage, Is.EqualTo(50));
        }

        [Test]
        public async Task GetDashboardAsync_ShouldIncludeProjects_WhenUserIsMember()
        {
            string ownerId = "owner-id";
            string memberId = "member-id";

            Project project = new Project
            {
                Id = 1,
                Name = "Test Project",
                OwnerId = ownerId,
                CreatedOn = DateTime.UtcNow
            };

            ProjectMember projectMember = new ProjectMember
            {
                ProjectId = project.Id,
                UserId = memberId,
                JoinedOn = DateTime.UtcNow
            };

            TaskItem task = new TaskItem
            {
                Id = 1,
                Title = "Test Task",
                ProjectId = project.Id,
                CreatedByUserId = ownerId,
                Status = TaskItemStatus.Completed
            };

            await context.Projects.AddAsync(project);
            await context.ProjectMembers.AddAsync(projectMember);
            await context.TaskItems.AddAsync(task);
            await context.SaveChangesAsync();

            var result =
                await homeService.GetDashboardAsync(memberId);

            Assert.That(result.ProjectsCount, Is.EqualTo(1));
            Assert.That(result.TasksCount, Is.EqualTo(1));
            Assert.That(result.CompletedTasksCount, Is.EqualTo(1));
            Assert.That(result.CompletionPercentage, Is.EqualTo(100));
        }

        [Test]
        public async Task GetDashboardAsync_ShouldReturnZeros_WhenUserHasNoProjects()
        {
            string userId = "user-without-projects";

            var result =
                await homeService.GetDashboardAsync(userId);

            Assert.That(result.ProjectsCount, Is.EqualTo(0));
            Assert.That(result.TasksCount, Is.EqualTo(0));
            Assert.That(result.CompletedTasksCount, Is.EqualTo(0));
            Assert.That(result.CompletionPercentage, Is.EqualTo(0));
        }
    }
}