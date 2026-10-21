using System.ComponentModel.DataAnnotations;

namespace Nhom2__WebAPI.Models.DTOs
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string Password { get; set; } = string.Empty;

        // Mã OTP (nếu đăng nhập theo luồng 2 bước hoặc nhập cùng lúc)
        public string? Otp { get; set; }
    }
}
