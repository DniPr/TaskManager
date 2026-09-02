using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManager.Models.Enums;

namespace TaskManager.Models
{
    public class TaskItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = null!;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required]
        public TaskItemStatus Status { get; set; } = TaskItemStatus.ToDo;

        [Required]
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime? Deadline { get; set; }

        public DateTime? CompletedOn { get; set; }


        [Required]
        [ForeignKey(nameof(Project))]
        public int ProjectId { get; set; }

        public virtual Project Project { get; set; } = null!;


        [ForeignKey(nameof(AssignedUser))]
        public string? AssignedUserId { get; set; }

        public virtual ApplicationUser? AssignedUser { get; set; }


        public virtual ICollection<Comment> Comments { get; set; }
            = new HashSet<Comment>();
    }
}