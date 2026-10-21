using Microsoft.AspNetCore.Identity;

namespace Nhom2__WebAPI.Models
{
    public class User : IdentityUser
    {
        public string? Initials { get; set; }
        public string? HoTen { get; set; }
        public string? MaSV { get; set; }
    }
}
