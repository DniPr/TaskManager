namespace TaskManager.ViewModels.ProjectMemberViewModels
{
    public class ProjectMembersPageViewModel
    {
        public int ProjectId { get; set; }

        public string ProjectName { get; set; } = null!;

        public bool IsOwner { get; set; }

        public IEnumerable<ProjectMemberViewModel> Members { get; set; } = new List<ProjectMemberViewModel>();
    }
}
