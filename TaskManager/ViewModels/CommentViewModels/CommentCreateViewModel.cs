using System.ComponentModel.DataAnnotations;

namespace TaskManager.ViewModels.CommentViewModels
{
    public class CommentCreateViewModel
    {
        public int TaskItemId { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 1)]
        public string Content { get; set; } = null!;
    }
}
