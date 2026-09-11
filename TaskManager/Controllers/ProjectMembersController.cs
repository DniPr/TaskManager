using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels.ProjectMemberViewModels;

namespace TaskManager.Controllers
{
    [Authorize]
    public class ProjectMembersController : Controller
    {
        private readonly IProjectMemberService projectMemberService;
        public ProjectMembersController(IProjectMemberService projectMemberService)
        {
            this.projectMemberService = projectMemberService;
        }


        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }


        [HttpGet]
        public async Task<IActionResult> Index(int projectId)
        {
            string userId = GetUserId();

            ProjectMembersPageViewModel? vmodel =
                await projectMemberService.GetMembersAsync(
                    projectId,
                    userId);

            if (vmodel == null)
            {
                return NotFound();
            }

            return View(vmodel);
        }


        [HttpGet]
        public IActionResult Add(int projectId)
        {
            ProjectMemberAddViewModel vmodel = new ProjectMemberAddViewModel
            {
                ProjectId = projectId
            };

            return View(vmodel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ProjectMemberAddViewModel vmodel)
        {
            if (!ModelState.IsValid)
            {
                return View(vmodel);
            }

            string userId = GetUserId();

            bool isAdded =
                await projectMemberService.AddMemberAsync(
                    vmodel,
                    userId);

            if (!isAdded)
            {
                ModelState.AddModelError(
                    nameof(vmodel.Email),
                    "The user could not be added. Check the email, membership, or your permissions.");

                return View(vmodel);
            }

            return RedirectToAction(
                nameof(Index),
                new { projectId = vmodel.ProjectId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int projectId,string memberUserId)
        {
            string userId = GetUserId();

            bool isRemoved =
                await projectMemberService.RemoveMemberAsync(
                    projectId,
                    memberUserId,
                    userId);

            if (!isRemoved)
            {
                return NotFound();
            }

            return RedirectToAction(
                nameof(Index),
                new { projectId });
        }
    }
}
