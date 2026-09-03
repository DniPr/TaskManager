namespace TaskManager.ViewModels.ProjectViewModels
{
    public class ProjectDetailsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? Deadline { get; set; }

        public string OwnerId { get; set; } = null!;

        public string OwnerName { get; set; } = null!;

        public int MembersCount { get; set; }

        public int TasksCount { get; set; }

        public int CompletedTasksCount { get; set; }
    }
}
