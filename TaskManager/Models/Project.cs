using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime? Deadline { get; set; }


        [Required]
        [ForeignKey(nameof(Owner))]
        public string OwnerId { get; set; } = null!;

        public virtual ApplicationUser Owner { get; set; } = null!;


        public virtual ICollection<ProjectMember> ProjectMembers { get; set; }
            = new HashSet<ProjectMember>();

        public virtual ICollection<TaskItem> Tasks { get; set; }
            = new HashSet<TaskItem>();
    }
}
