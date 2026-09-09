using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels.TaskViewModels;

namespace TaskManager.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ITaskService taskService;
        public TasksController(ITaskService taskService)
        {
            this.taskService = taskService;
        }
        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }


        [HttpGet]
        public async Task<IActionResult> Index(int projectId)
        {
            string userId = GetUserId();

            TaskIndexPageViewModel? vmodel =
                await taskService.GetAllByProjectAsync(projectId, userId);

            if (vmodel == null)
            {
                return NotFound();
            }

            return View(vmodel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            string userId = GetUserId();

            TaskDetailsViewModel? task =
                await taskService.GetDetailsAsync(id, userId);

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }


        [HttpGet]
        public IActionResult Create(int projectId)
        {
            TaskCreateViewModel vmodel = new TaskCreateViewModel
            {
                ProjectId = projectId
            };

            return View(vmodel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskCreateViewModel vmodel)
        {
            if (!ModelState.IsValid)
            {
                return View(vmodel);
            }

            string userId = GetUserId();

            try
            {
                await taskService.CreateAsync(vmodel, userId);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException)
            {
                ModelState.AddModelError(
                    nameof(vmodel.AssignedUserId),
                    "The selected user is not a member of this project.");

                return View(vmodel);
            }

            return RedirectToAction(
                nameof(Index),
                new { projectId = vmodel.ProjectId });
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            string userId = GetUserId();

            TaskEditViewModel? task =
                await taskService.GetForEditAsync(id, userId);

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,TaskEditViewModel vmodel)
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
                await taskService.EditAsync(id, vmodel, userId);

            if (!isEdited)
            {
                return NotFound();
            }

            return RedirectToAction(
                nameof(Details),
                new { id });
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            string userId = GetUserId();

            TaskDetailsViewModel? task =
                await taskService.GetDetailsAsync(id, userId);

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,int projectId)
        {
            string userId = GetUserId();

            bool isDeleted =
                await taskService.DeleteAsync(id, userId);

            if (!isDeleted)
            {
                return NotFound();
            }

            return RedirectToAction(
                nameof(Index),
                new { projectId });
        }
    }
}
