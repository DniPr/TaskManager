using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = null!;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Project> OwnedProjects { get; set; }
            = new HashSet<Project>();

        public virtual ICollection<ProjectMember> ProjectMemberships { get; set; }
            = new HashSet<ProjectMember>();

        public virtual ICollection<TaskItem> AssignedTasks { get; set; }
            = new HashSet<TaskItem>();

        public virtual ICollection<Comment> Comments { get; set; }
            = new HashSet<Comment>();
    }
}