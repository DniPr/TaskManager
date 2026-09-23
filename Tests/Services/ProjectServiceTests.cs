using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Services;
using TaskManager.ViewModels.ProjectViewModels;

namespace TaskManager.Tests.Services
{
    [TestFixture]
    public class ProjectServiceTests
    {
        private ApplicationDbContext context = null!;
        private ProjectService projectService = null!;

        [SetUp]
        public void SetUp()
        {
            DbContextOptions<ApplicationDbContext> options =
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;

            context = new ApplicationDbContext(options);

            projectService = new ProjectService(context);
        }

        [TearDown]
        public void TearDown()
        {
            context.Dispose();
        }

        [Test]
        public async Task CreateAsync_ShouldCreateProject_WithCorrectOwner()
        {
            string ownerId = "owner-id";

            ProjectCreateViewModel vmodel = new ProjectCreateViewModel
            {
                Name = "Test Project",
                Description = "Test Description",
                Deadline = DateTime.UtcNow.AddDays(10)
            };

            await projectService.CreateAsync(vmodel, ownerId);

            var project = await context.Projects.FirstOrDefaultAsync();

            Assert.That(project, Is.Not.Null);
            Assert.That(project!.Name, Is.EqualTo("Test Project"));
            Assert.That(project.Description, Is.EqualTo("Test Description"));
            Assert.That(project.OwnerId, Is.EqualTo(ownerId));
        }

        [Test]
        public async Task EditAsync_ShouldUpdateProject_WhenUserIsOwner()
        {
            string ownerId = "owner-id";

            Project project = new Project
            {
                Id = 1,
                Name = "Old Name",
                Description = "Old Description",
                OwnerId = ownerId,
                CreatedOn = DateTime.UtcNow
            };

            await context.Projects.AddAsync(project);
            await context.SaveChangesAsync();

            ProjectEditViewModel vmodel = new ProjectEditViewModel
            {
                Id = project.Id,
                Name = "Updated Name",
                Description = "Updated Description",
                Deadline = DateTime.UtcNow.AddDays(5)
            };

            bool result = await projectService.EditAsync(project.Id,vmodel,ownerId);

            Assert.That(result, Is.True);

            Project? updatedProject =
                await context.Projects.FindAsync(project.Id);

            Assert.That(updatedProject, Is.Not.Null);
            Assert.That(updatedProject!.Name, Is.EqualTo("Updated Name"));
            Assert.That(updatedProject.Description, Is.EqualTo("Updated Description"));
        }

        [Test]
        public async Task EditAsync_ShouldReturnFalse_WhenUserIsNotOwner()
        {
            string ownerId = "owner-id";
            string otherUserId = "other-user-id";

            Project project = new Project
            {
                Id = 1,
                Name = "Original Name",
                Description = "Original Description",
                OwnerId = ownerId,
                CreatedOn = DateTime.UtcNow
            };

            await context.Projects.AddAsync(project);
            await context.SaveChangesAsync();

            ProjectEditViewModel vmodel = new ProjectEditViewModel
            {
                Id = project.Id,
                Name = "Changed Name",
                Description = "Changed Description",
                Deadline = DateTime.UtcNow.AddDays(5)
            };

            bool result = await projectService.EditAsync(project.Id,vmodel,otherUserId);

            Assert.That(result, Is.False);

            Project? unchangedProject =
                await context.Projects.FindAsync(project.Id);

            Assert.That(unchangedProject, Is.Not.Null);
            Assert.That(unchangedProject!.Name, Is.EqualTo("Original Name"));
            Assert.That(unchangedProject.Description,Is.EqualTo("Original Description"));
        }

        [Test]
        public async Task DeleteAsync_ShouldDeleteProject_WhenUserIsOwner()
        {
            string ownerId = "owner-id";

            Project project = new Project
            {
                Id = 1,
                Name = "Project To Delete",
                Description = "Test Description",
                OwnerId = ownerId,
                CreatedOn = DateTime.UtcNow
            };

            await context.Projects.AddAsync(project);
            await context.SaveChangesAsync();

            bool result = await projectService.DeleteAsync(project.Id,ownerId);

            Assert.That(result, Is.True);

            Project? deletedProject =
                await context.Projects.FindAsync(project.Id);

            Assert.That(deletedProject, Is.Null);
        }
    }
}