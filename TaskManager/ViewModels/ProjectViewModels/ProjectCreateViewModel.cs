using System.ComponentModel.DataAnnotations;

namespace TaskManager.ViewModels.ProjectViewModels
{
    public class ProjectCreateViewModel
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; } = null!;

        [StringLength(1000)]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Deadline { get; set; }
    }
}
