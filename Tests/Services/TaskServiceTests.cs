using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Models.Enums;
using TaskManager.Services;
using TaskManager.ViewModels.TaskViewModels;

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
        public async Task CreateAsync_ShouldCreateTask_WhenUserHasProjectAccess()
        {
            string ownerId = "owner-id";

            Project project = new Project
            {
                Id = 1,
                Name = "Test Project",
                OwnerId = ownerId,
                CreatedOn = DateTime.UtcNow
            };

            await context.Projects.AddAsync(project);
            await context.SaveChangesAsync();

            TaskCreateViewModel vmodel = new TaskCreateViewModel
            {
                Title = "New Task",
                Description = "Test Description",
                Priority = TaskPriority.High,
                Deadline = DateTime.UtcNow.AddDays(5),
                ProjectId = project.Id,
                AssignedUserId = null
            };

            await taskService.CreateAsync(vmodel,ownerId);

            TaskItem? createdTask =
                await context.TaskItems
                    .FirstOrDefaultAsync();

            Assert.That(createdTask, Is.Not.Null);
            Assert.That(createdTask!.Title, Is.EqualTo("New Task"));
            Assert.That(createdTask.Description, Is.EqualTo("Test Description"));
            Assert.That(createdTask.Priority, Is.EqualTo(TaskPriority.High));
            Assert.That(createdTask.ProjectId, Is.EqualTo(project.Id));
            Assert.That(createdTask.CreatedByUserId, Is.EqualTo(ownerId));
            Assert.That(createdTask.Status, Is.EqualTo(TaskItemStatus.ToDo));
        }

        [Test]
        public void CreateAsync_ShouldThrowUnauthorizedAccessException_WhenUserHasNoProjectAccess()
        {
            string ownerId = "owner-id";
            string otherUserId = "other-user-id";

            Project project = new Project
            {
                Id = 1,
                Name = "Test Project",
                OwnerId = ownerId,
                CreatedOn = DateTime.UtcNow
            };

            context.Projects.Add(project);
            context.SaveChanges();

            TaskCreateViewModel vmodel = new TaskCreateViewModel
            {
                Title = "Unauthorized Task",
                Description = "Should not be created",
                Priority = TaskPriority.Medium,
                ProjectId = project.Id,
                AssignedUserId = null
            };

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await taskService.CreateAsync(vmodel,otherUserId));
        }

        [Test]
        public async Task EditAsync_ShouldUpdateTask_WhenUserCanModify()
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
                Title = "Old Title",
                Description = "Old Description",
                ProjectId = project.Id,
                Project = project,
                CreatedByUserId = "creator-id",
                Status = TaskItemStatus.ToDo,
                Priority = TaskPriority.Low
            };

            await context.Projects.AddAsync(project);
            await context.TaskItems.AddAsync(task);
            await context.SaveChangesAsync();

            TaskEditViewModel vmodel = new TaskEditViewModel
            {
                Id = task.Id,
                ProjectId = project.Id,
                Title = "Updated Title",
                Description = "Updated Description",
                Status = TaskItemStatus.InProgress,
                Priority = TaskPriority.High,
                Deadline = DateTime.UtcNow.AddDays(3),
                AssignedUserId = null
            };

            bool result = await taskService.EditAsync(task.Id,vmodel,ownerId);

            Assert.That(result, Is.True);

            TaskItem? updatedTask = await context.TaskItems.FindAsync(task.Id);

            Assert.That(updatedTask, Is.Not.Null);
            Assert.That(updatedTask!.Title, Is.EqualTo("Updated Title"));
            Assert.That(updatedTask.Description, Is.EqualTo("Updated Description"));
            Assert.That(updatedTask.Status, Is.EqualTo(TaskItemStatus.InProgress));
            Assert.That(updatedTask.Priority, Is.EqualTo(TaskPriority.High));
            Assert.That(updatedTask.Deadline, Is.Not.Null);
        }

        [Test]
        public async Task DeleteAsync_ShouldDeleteTask_WhenUserCanModify()
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
                Title = "Task To Delete",
                ProjectId = project.Id,
                Project = project,
                CreatedByUserId = "creator-id",
                Status = TaskItemStatus.ToDo,
                Priority = TaskPriority.Medium
            };

            await context.Projects.AddAsync(project);
            await context.TaskItems.AddAsync(task);
            await context.SaveChangesAsync();

            bool result = await taskService.DeleteAsync(task.Id,ownerId);

            Assert.That(result, Is.True);

            TaskItem? deletedTask = await context.TaskItems.FindAsync(task.Id);

            Assert.That(deletedTask, Is.Null);
        }
    }
}