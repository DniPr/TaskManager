using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels.ProjectViewModels;

namespace TaskManager.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly IProjectService projectService;
        public ProjectsController(IProjectService projectService)
        {
            this.projectService = projectService;
        }
        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string userId = GetUserId();

            IEnumerable<ProjectIndexViewModel> projects =
                await projectService.GetAllAsync(userId);

            return View(projects);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            string userId = GetUserId();

            ProjectDetailsViewModel? project =
                await projectService.GetDetailsAsync(id, userId);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectCreateViewModel vmodel)
        {
            if (!ModelState.IsValid)
            {
                return View(vmodel);
            }

            string userId = GetUserId();

            await projectService.CreateAsync(vmodel, userId);

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            string userId = GetUserId();

            ProjectEditViewModel? project =
                await projectService.GetForEditAsync(id, userId);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProjectEditViewModel vmodel)
        {
            if (id != vmodel.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(vmodel);
            }

            string userId = GetUserId();

            bool isEdited =
                await projectService.EditAsync(id, vmodel, userId);

            if (!isEdited)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Details), new { id });
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            string userId = GetUserId();

            ProjectDetailsViewModel? project =
                await projectService.GetDetailsAsync(id, userId);

            if (project == null)
            {
                return NotFound();
            }

            if (project.OwnerId != userId)
            {
                return Forbid();
            }

            return View(project);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string userId = GetUserId();

            bool isDeleted =
                await projectService.DeleteAsync(id, userId);

            if (!isDeleted)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
