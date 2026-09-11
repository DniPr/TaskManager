namespace TaskManager.ViewModels.ProjectMemberViewModels
{
    public class ProjectMemberViewModel
    {
        public string UserId { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public DateTime JoinedOn { get; set; }
    }
}
