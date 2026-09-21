using TaskManager.Models.Enums;

namespace TaskManager.ViewModels.TaskViewModels
{
    public class TaskStatusUpdateViewModel
    {
        public int TaskId { get; set; }

        public TaskItemStatus Status { get; set; }
    }
}