using Microsoft.AspNetCore.Identity;

namespace DoAnLapTrinhWeb.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Tên hiển thị / Tên đăng nhập của người dùng
        public string? FullName { get; set; }
    }
}
