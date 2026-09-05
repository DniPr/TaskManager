using System.ComponentModel.DataAnnotations;
using TaskManager.Models.Enums;

namespace TaskManager.ViewModels.TaskViewModels
{
    public class TaskEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Title { get; set; } = null!;

        [StringLength(2000)]
        public string? Description { get; set; }

        public TaskItemStatus Status { get; set; }

        public TaskPriority Priority { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Deadline { get; set; }

        [Required]
        public int ProjectId { get; set; }

        public string? AssignedUserId { get; set; }
    }
}
