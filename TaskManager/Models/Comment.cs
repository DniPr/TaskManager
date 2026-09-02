using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = null!;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;


        [Required]
        [ForeignKey(nameof(TaskItem))]
        public int TaskItemId { get; set; }

        public virtual TaskItem TaskItem { get; set; } = null!;


        [Required]
        [ForeignKey(nameof(Author))]
        public string AuthorId { get; set; } = null!;

        public virtual ApplicationUser Author { get; set; } = null!;
    }
}