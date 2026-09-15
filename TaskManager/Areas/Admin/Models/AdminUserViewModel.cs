namespace TaskManager.Areas.Admin.Models
{
    public class AdminUserViewModel
    {
        public string Id { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public DateTime CreatedOn { get; set; }

        public string Role { get; set; } = null!;
    }
}