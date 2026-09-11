using TaskManager.Models.Enums;

namespace TaskManager.ViewModels.TaskViewModels
{
    public class TaskIndexViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public TaskItemStatus Status { get; set; }

        public TaskPriority Priority { get; set; }

        public DateTime? Deadline { get; set; }

        public string? AssignedUserName { get; set; }
    }
}
