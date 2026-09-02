using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    public class ProjectMember
    {
        [ForeignKey(nameof(Project))]
        public int ProjectId { get; set; }

        public virtual Project Project { get; set; } = null!;

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;

        public virtual ApplicationUser User { get; set; } = null!;

        public DateTime JoinedOn { get; set; } = DateTime.UtcNow;
    }
}