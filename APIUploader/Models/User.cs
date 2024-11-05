namespace WebApplication1.Models
{
    public class User
    {
        public string? Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public long StorageUsed { get; set; } = 0;
    }
}
