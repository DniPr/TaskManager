namespace TaskManager.ViewModels.HomeViewModels
{
    public class HomeDashboardViewModel
    {
        public int ProjectsCount { get; set; }

        public int TasksCount { get; set; }

        public int CompletedTasksCount { get; set; }

        public int CompletionPercentage { get; set; }
    }
}