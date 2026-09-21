using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.Models.Enums;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels.TaskViewModels;

namespace TaskManager.Controllers.Api
{
    [ApiController]
    [Route("api/tasks")]
    [Authorize]
    public class TasksApiController : ControllerBase
    {
        private readonly ITaskService taskService;

        public TasksApiController(ITaskService taskService)
        {
            this.taskService = taskService;
        }

        [HttpPut("status")]
        public async Task<IActionResult> UpdateStatus([FromBody] TaskStatusUpdateViewModel vmodel)
        {

            if (!Enum.IsDefined(typeof(TaskItemStatus), vmodel.Status))
            {
                return BadRequest(new
                {
                    message = "Invalid task status."
                });
            }

            string userId =
                User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            bool isUpdated =
                await taskService.UpdateStatusAsync(vmodel.TaskId,vmodel.Status,userId);

            if (!isUpdated)
            {
                return BadRequest(new
                {
                    message = "Task status could not be updated."
                });
            }

            return Ok(new
            {
                taskId = vmodel.TaskId,
                status = vmodel.Status.ToString()
            });
        }
    }
}