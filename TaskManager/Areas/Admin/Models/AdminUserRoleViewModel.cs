using System.ComponentModel.DataAnnotations;

namespace TaskManager.Areas.Admin.Models
{
    public class AdminUserRoleViewModel
    {
        public string UserId { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        [Required]
        public string Role { get; set; } = null!;
    }
}