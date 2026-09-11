using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels.CommentViewModels;

namespace TaskManager.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly ICommentService commentService;
        public CommentsController(ICommentService commentService)
        {
            this.commentService = commentService;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CommentCreateViewModel vmodel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(
                    "Details",
                    "Tasks",
                    new { id = vmodel.TaskItemId });
            }

            string userId = GetUserId();

            bool isCreated =
                await commentService.CreateAsync(vmodel, userId);

            if (!isCreated)
            {
                return NotFound();
            }

            return RedirectToAction(
                "Details",
                "Tasks",
                new { id = vmodel.TaskItemId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id,int taskId)
        {
            string userId = GetUserId();

            bool isDeleted =
                await commentService.DeleteAsync(id, userId);

            if (!isDeleted)
            {
                return NotFound();
            }

            return RedirectToAction(
                "Details",
                "Tasks",
                new { id = taskId });
        }
    }
}
