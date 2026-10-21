using System.ComponentModel.DataAnnotations;

namespace Nhom2__WebAPI.Models.DTOs
{
    public class RequestOtpModel
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string Password { get; set; } = string.Empty;
    }

    public class VerifyOtpModel
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã OTP không được để trống")]
        public string Otp { get; set; } = string.Empty;
    }
}
