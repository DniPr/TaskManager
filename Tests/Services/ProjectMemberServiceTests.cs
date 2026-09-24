using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Services;
using TaskManager.ViewModels.ProjectMemberViewModels;

namespace TaskManager.Tests.Services
{
    [TestFixture]
    public class ProjectMemberServiceTests
    {
        private ApplicationDbContext context = null!;
        private ProjectMemberService projectMemberService = null!;

        [SetUp]
        public void SetUp()
        {
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;

            context = new ApplicationDbContext(options);

            projectMemberService =
                new ProjectMemberService(context);
        }

        [TearDown]
        public void TearDown()
        {
            context.Dispose();
        }

        [Test]
        public async Task AddMemberAsync_ShouldAddMember_WhenUserIsProjectOwner()
        {
            string ownerId = "owner-id";
            string memberId = "member-id";

            ApplicationUser member = new ApplicationUser
            {
                Id = memberId,
                UserName = "member@test.com",
                Email = "member@test.com",
                FirstName = "Test",
                LastName = "Member"
            };

            Project project = new Project
            {
                Id = 1,
                Name = "Test Project",
                OwnerId = ownerId,
                CreatedOn = DateTime.UtcNow
            };

            await context.Users.AddAsync(member);
            await context.Projects.AddAsync(project);
            await context.SaveChangesAsync();

            ProjectMemberAddViewModel vmodel =
                new ProjectMemberAddViewModel
                {
                    ProjectId = project.Id,
                    Email = member.Email!
                };

            bool result = await projectMemberService.AddMemberAsync(vmodel,ownerId);

            Assert.That(result, Is.True);

            ProjectMember? projectMember =
                await context.ProjectMembers
                    .FirstOrDefaultAsync(pm =>
                        pm.ProjectId == project.Id &&
                        pm.UserId == memberId);

            Assert.That(projectMember, Is.Not.Null);
        }

        [Test]
        public async Task AddMemberAsync_ShouldReturnFalse_WhenMemberAlreadyExists()
        {
            string ownerId = "owner-id";
            string memberId = "member-id";

            ApplicationUser member = new ApplicationUser
            {
                Id = memberId,
                UserName = "member@test.com",
                Email = "member@test.com",
                FirstName = "Test",
                LastName = "Member"
            };

            Project project = new Project
            {
                Id = 1,
                Name = "Test Project",
                OwnerId = ownerId,
                CreatedOn = DateTime.UtcNow
            };

            ProjectMember existingMember = new ProjectMember
            {
                ProjectId = project.Id,
                UserId = memberId,
                JoinedOn = DateTime.UtcNow
            };

            await context.Users.AddAsync(member);
            await context.Projects.AddAsync(project);
            await context.ProjectMembers.AddAsync(existingMember);
            await context.SaveChangesAsync();

            ProjectMemberAddViewModel vmodel =
                new ProjectMemberAddViewModel
                {
                    ProjectId = project.Id,
                    Email = member.Email!
                };

            bool result = await projectMemberService.AddMemberAsync(vmodel,ownerId);

            Assert.That(result, Is.False);

            int membersCount =
                await context.ProjectMembers
                    .CountAsync(pm =>
                        pm.ProjectId == project.Id &&
                        pm.UserId == memberId);

            Assert.That(membersCount, Is.EqualTo(1));
        }

        [Test]
        public async Task RemoveMemberAsync_ShouldRemoveMember_WhenUserIsProjectOwner()
        {
            string ownerId = "owner-id";
            string memberId = "member-id";

            ApplicationUser member = new ApplicationUser
            {
                Id = memberId,
                UserName = "member@test.com",
                Email = "member@test.com",
                FirstName = "Test",
                LastName = "Member"
            };

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

            await context.Users.AddAsync(member);
            await context.Projects.AddAsync(project);
            await context.ProjectMembers.AddAsync(projectMember);
            await context.SaveChangesAsync();

            bool result = await projectMemberService.RemoveMemberAsync(project.Id,memberId,ownerId);

            Assert.That(result, Is.True);

            ProjectMember? removedMember =
                await context.ProjectMembers
                    .FirstOrDefaultAsync(pm =>
                        pm.ProjectId == project.Id &&
                        pm.UserId == memberId);

            Assert.That(removedMember, Is.Null);
        }
    }
}