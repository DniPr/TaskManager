using TaskManager.Models.Enums;

namespace TaskManager.ViewModels.TaskViewModels
{
    public class TaskDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public TaskItemStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime? CompletedOn { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = null!;
        public string? AssignedUserId { get; set; }
        public string? AssignedUserName { get; set; }
        public int CommentsCount { get; set; }
    }
}
