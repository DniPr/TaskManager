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
        public async Task<IActionResult> Create(int projectId)
        {
            string userId = GetUserId();

            IEnumerable<TaskAssigneeViewModel> assignees =
                await taskService.GetProjectAssigneesAsync(projectId, userId);

            TaskCreateViewModel vmodel = new TaskCreateViewModel
            {
                ProjectId = projectId,
                Assignees = assignees
            };

            return View(vmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskCreateViewModel vmodel)
        {
            string userId = GetUserId();

            if (!ModelState.IsValid)
            {
                vmodel.Assignees =
                    await taskService.GetProjectAssigneesAsync(
                        vmodel.ProjectId,
                        userId);

                return View(vmodel);
            }

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

                vmodel.Assignees =
                    await taskService.GetProjectAssigneesAsync(
                        vmodel.ProjectId,
                        userId);

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

            TaskEditViewModel? vmodel =
                await taskService.GetForEditAsync(id, userId);

            if (vmodel == null)
            {
                return NotFound();
            }

            vmodel.Assignees =
                await taskService.GetProjectAssigneesAsync(
                    vmodel.ProjectId,
                    userId);

            return View(vmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,TaskEditViewModel vmodel)
        {
            if (id != vmodel.Id)
            {
                return BadRequest();
            }

            string userId = GetUserId();

            if (!ModelState.IsValid)
            {
                vmodel.Assignees =
                    await taskService.GetProjectAssigneesAsync(
                        vmodel.ProjectId,
                        userId);

                return View(vmodel);
            }

            bool isEdited =
                await taskService.EditAsync(
                    id,
                    vmodel,
                    userId);

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
