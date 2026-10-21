using System.ComponentModel.DataAnnotations;

namespace Nhom2__WebAPI.Models.DTOs
{
    public class RegistrationModel
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
        public string Password { get; set; } = string.Empty;

        public string? Initials { get; set; }
        public string? Role { get; set; } // "Admin" hoặc "SinhVien" (mặc định "SinhVien")

        // Thông tin sinh viên (nếu role là SinhVien)
        public string? MaSV { get; set; }
        public string? HoTen { get; set; }
        public string? Lop { get; set; }
        public string? Khoa { get; set; }
        public string? SoDienThoai { get; set; }
    }
}
