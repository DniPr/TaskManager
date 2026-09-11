using System.ComponentModel.DataAnnotations;

namespace TaskManager.ViewModels.ProjectMemberViewModels
{
    public class ProjectMemberAddViewModel
    {
        public int ProjectId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
