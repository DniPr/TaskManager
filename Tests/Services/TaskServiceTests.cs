using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Models.Enums;
using TaskManager.Services;

namespace TaskManager.Tests.Services
{
    [TestFixture]
    public class TaskServiceTests
    {
        private ApplicationDbContext context = null!;
        private TaskService taskService = null!;

        [SetUp]
        public void SetUp()
        {
            DbContextOptions<ApplicationDbContext> options =new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;

            context = new ApplicationDbContext(options);

            taskService = new TaskService(context);
        }

        [TearDown]
        public void TearDown()
        {
            context.Dispose();
        }

        [Test]
        public async Task UpdateStatusAsync_ShouldReturnFalse_WhenTaskDoesNotExist()
        {
            int invalidTaskId = 999;
            string userId = "user-id";

            bool result = await taskService.UpdateStatusAsync(invalidTaskId,TaskManager.Models.Enums.TaskItemStatus.InProgress,userId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateStatusAsync_ShouldUpdateStatus_WhenUserIsProjectOwner()
        {
            string ownerId = "owner-id";

            Project project = new Project
            {
                Id = 1,
                Name = "Test Project",
                OwnerId = ownerId
            };

            TaskItem task = new TaskItem
            {
                Id = 1,
                Title = "Test Task",
                ProjectId = project.Id,
                Project = project,
                CreatedByUserId = "another-user-id",
                Status = TaskItemStatus.ToDo
            };

            await context.Projects.AddAsync(project);
            await context.TaskItems.AddAsync(task);
            await context.SaveChangesAsync();

            bool result = await taskService.UpdateStatusAsync(task.Id,TaskItemStatus.InProgress,ownerId);

            Assert.That(result, Is.True);
            Assert.That(task.Status, Is.EqualTo(TaskItemStatus.InProgress));
        }

        [Test]
        public async Task UpdateStatusAsync_ShouldUpdateStatus_WhenUserIsTaskCreator()
        {
            string ownerId = "owner-id";
            string creatorId = "creator-id";

            Project project = new Project
            {
                Id = 1,
                Name = "Test Project",
                OwnerId = ownerId
            };

            TaskItem task = new TaskItem
            {
                Id = 1,
                Title = "Test Task",
                ProjectId = project.Id,
                Project = project,
                CreatedByUserId = creatorId,
                Status = TaskItemStatus.ToDo
            };

            await context.Projects.AddAsync(project);
            await context.TaskItems.AddAsync(task);
            await context.SaveChangesAsync();

            bool result = await taskService.UpdateStatusAsync(task.Id,TaskItemStatus.InProgress,creatorId);

            Assert.That(result, Is.True);
            Assert.That(task.Status, Is.EqualTo(TaskItemStatus.InProgress));
        }

        [Test]
        public async Task UpdateStatusAsync_ShouldReturnFalse_WhenUserCannotModifyTask()
        {
            string ownerId = "owner-id";
            string creatorId = "creator-id";
            string unauthorizedUserId = "unauthorized-user-id";

            Project project = new Project
            {
                Id = 1,
                Name = "Test Project",
                OwnerId = ownerId
            };

            TaskItem task = new TaskItem
            {
                Id = 1,
                Title = "Test Task",
                ProjectId = project.Id,
                Project = project,
                CreatedByUserId = creatorId,
                Status = TaskItemStatus.ToDo
            };

            await context.Projects.AddAsync(project);
            await context.TaskItems.AddAsync(task);
            await context.SaveChangesAsync();

            bool result = await taskService.UpdateStatusAsync(task.Id,TaskItemStatus.InProgress,unauthorizedUserId);

            Assert.That(result, Is.False);
            Assert.That(task.Status, Is.EqualTo(TaskItemStatus.ToDo));
        }

        [Test]
        public async Task UpdateStatusAsync_ShouldSetCompletedOn_WhenStatusBecomesCompleted()
        {
            string ownerId = "owner-id";

            Project project = new Project
            {
                Id = 1,
                Name = "Test Project",
                OwnerId = ownerId
            };

            TaskItem task = new TaskItem
            {
                Id = 1,
                Title = "Test Task",
                ProjectId = project.Id,
                Project = project,
                CreatedByUserId = "creator-id",
                Status = TaskItemStatus.InProgress,
                CompletedOn = null
            };

            await context.Projects.AddAsync(project);
            await context.TaskItems.AddAsync(task);
            await context.SaveChangesAsync();

            bool result = await taskService.UpdateStatusAsync(task.Id,TaskItemStatus.Completed,ownerId);

            Assert.That(result, Is.True);
            Assert.That(task.Status, Is.EqualTo(TaskItemStatus.Completed));
            Assert.That(task.CompletedOn, Is.Not.Null);
        }

        [Test]
        public async Task UpdateStatusAsync_ShouldClearCompletedOn_WhenStatusChangesFromCompleted()
        {
            string ownerId = "owner-id";

            Project project = new Project
            {
                Id = 1,
                Name = "Test Project",
                OwnerId = ownerId
            };

            TaskItem task = new TaskItem
            {
                Id = 1,
                Title = "Test Task",
                ProjectId = project.Id,
                Project = project,
                CreatedByUserId = "creator-id",
                Status = TaskItemStatus.Completed,
                CompletedOn = DateTime.UtcNow
            };

            await context.Projects.AddAsync(project);
            await context.TaskItems.AddAsync(task);
            await context.SaveChangesAsync();

            bool result = await taskService.UpdateStatusAsync(task.Id,TaskItemStatus.InProgress,ownerId);

            Assert.That(result, Is.True);
            Assert.That(task.Status, Is.EqualTo(TaskItemStatus.InProgress));
            Assert.That(task.CompletedOn, Is.Null);
        }
    }
}