namespace TaskManager.ViewModels.TaskViewModels
{
    public class TaskIndexPageViewModel
    {
        public int ProjectId { get; set; }

        public string ProjectName { get; set; } = null!;

        public IEnumerable<TaskIndexViewModel> Tasks { get; set; } = new List<TaskIndexViewModel>();
    }
}
