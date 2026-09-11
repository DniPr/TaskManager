using TaskManager.ViewModels.CommentViewModels;

namespace TaskManager.Services.Interfaces
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentViewModel>?> GetAllByTaskAsync(int taskId,string userId);

        Task<bool> CreateAsync(CommentCreateViewModel vmodel,string userId);

        Task<bool> DeleteAsync(int id,string userId);
    }
}
