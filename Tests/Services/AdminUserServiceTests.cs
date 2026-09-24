using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using TaskManager.Areas.Admin.Models;
using TaskManager.Areas.Admin.Services;
using TaskManager.Common;
using TaskManager.Models;

namespace TaskManager.Tests.Services
{
    [TestFixture]
    public class AdminUserServiceTests
    {
        private Mock<UserManager<ApplicationUser>> userManagerMock = null!;
        private AdminUserService adminUserService = null!;

        [SetUp]
        public void SetUp()
        {
            Mock<IUserStore<ApplicationUser>> userStoreMock = new Mock<IUserStore<ApplicationUser>>();

            userManagerMock = new Mock<UserManager<ApplicationUser>>(
                    userStoreMock.Object,
                    null!,
                    null!,
                    null!,
                    null!,
                    null!,
                    null!,
                    null!,
                    null!);

            adminUserService = new AdminUserService(userManagerMock.Object);
        }

        [Test]
        public async Task ChangeUserRoleAsync_ShouldChangeUserToAdmin()
        {
            string userId = "user-id";
            string currentAdminId = "admin-id";

            ApplicationUser user = new ApplicationUser
            {
                Id = userId,
                FirstName = "Test",
                LastName = "User",
                Email = "user@test.com"
            };

            AdminUserRoleViewModel vmodel =
                new AdminUserRoleViewModel
                {
                    UserId = userId,
                    Role = RoleConstants.Admin
                };

            userManagerMock
                .Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync(user);

            userManagerMock
                .Setup(um => um.GetRolesAsync(user))
                .ReturnsAsync(new List<string>{RoleConstants.User});

            userManagerMock
                .Setup(um => um.RemoveFromRolesAsync(
                    user,
                    It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(IdentityResult.Success);

            userManagerMock
                .Setup(um => um.AddToRoleAsync(
                    user,
                    RoleConstants.Admin))
                .ReturnsAsync(IdentityResult.Success);

            bool result =
                await adminUserService.ChangeUserRoleAsync(vmodel,currentAdminId);

            Assert.That(result, Is.True);

            userManagerMock.Verify(
                um => um.AddToRoleAsync(
                    user,
                    RoleConstants.Admin),
                Times.Once);
        }

        [Test]
        public async Task ChangeUserRoleAsync_ShouldChangeAdminToUser_WhenMoreThanOneAdminExists()
        {
            string userId = "admin-user-id";
            string currentAdminId = "current-admin-id";

            ApplicationUser user = new ApplicationUser
            {
                Id = userId,
                FirstName = "Test",
                LastName = "Admin",
                Email = "admin@test.com"
            };

            ApplicationUser anotherAdmin = new ApplicationUser
            {
                Id = "another-admin-id",
                FirstName = "Another",
                LastName = "Admin",
                Email = "another@test.com"
            };

            AdminUserRoleViewModel vmodel =
                new AdminUserRoleViewModel
                {
                    UserId = userId,
                    Role = RoleConstants.User
                };

            userManagerMock
                .Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync(user);

            userManagerMock
                .Setup(um => um.GetRolesAsync(user))
                .ReturnsAsync(new List<string>{RoleConstants.Admin});

            userManagerMock
                .Setup(um => um.GetUsersInRoleAsync(RoleConstants.Admin))
                .ReturnsAsync(new List<ApplicationUser>{user,anotherAdmin});

            userManagerMock
                .Setup(um => um.RemoveFromRolesAsync(
                    user,
                    It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(IdentityResult.Success);

            userManagerMock
                .Setup(um => um.AddToRoleAsync(
                    user,
                    RoleConstants.User))
                .ReturnsAsync(IdentityResult.Success);

            bool result =
                await adminUserService.ChangeUserRoleAsync(vmodel,currentAdminId);

            Assert.That(result, Is.True);

            userManagerMock.Verify(
                um => um.AddToRoleAsync(
                    user,
                    RoleConstants.User),
                Times.Once);
        }

        [Test]
        public async Task ChangeUserRoleAsync_ShouldReturnFalse_WhenDemotingLastAdmin()
        {
            string adminId = "admin-id";

            ApplicationUser admin = new ApplicationUser
            {
                Id = adminId,
                FirstName = "Last",
                LastName = "Admin",
                Email = "admin@test.com"
            };

            AdminUserRoleViewModel vmodel =
                new AdminUserRoleViewModel
                {
                    UserId = adminId,
                    Role = RoleConstants.User
                };

            userManagerMock
                .Setup(um => um.FindByIdAsync(adminId))
                .ReturnsAsync(admin);

            userManagerMock
                .Setup(um => um.GetRolesAsync(admin))
                .ReturnsAsync(new List<string>{RoleConstants.Admin});

            userManagerMock
                .Setup(um => um.GetUsersInRoleAsync(RoleConstants.Admin))
                .ReturnsAsync(new List<ApplicationUser>{admin});

            bool result =
                await adminUserService.ChangeUserRoleAsync(vmodel,adminId);

            Assert.That(result, Is.False);

            userManagerMock.Verify(
                um => um.AddToRoleAsync(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<string>()),
                Times.Never);

            userManagerMock.Verify(
                um => um.RemoveFromRolesAsync(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<IEnumerable<string>>()),
                Times.Never);
        }
    }
}