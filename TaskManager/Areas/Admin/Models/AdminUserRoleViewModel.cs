using System.ComponentModel.DataAnnotations;

namespace TaskManager.Areas.Admin.Models
{
    public class AdminUserRoleViewModel
    {
        public string UserId { get; set; } = null!;

        public string? FullName { get; set; }

        public string? Email { get; set; }

        [Required]
        public string Role { get; set; } = null!;
    }
}