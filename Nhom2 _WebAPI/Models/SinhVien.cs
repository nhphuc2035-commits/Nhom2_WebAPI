using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nhom2__WebAPI.Models
{
    public class SinhVien
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string MaSV { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? SoDienThoai { get; set; }

        [MaxLength(50)]
        public string? Lop { get; set; }

        [MaxLength(100)]
        public string? Khoa { get; set; }

        public string? AnhDaiDien { get; set; }

        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.UtcNow;

        public bool TrangThai { get; set; } = true;

        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<SinhVienLopHocPhan> SinhVienLopHocPhans { get; set; } = new List<SinhVienLopHocPhan>();

        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<DiemDanh> DanhSachDiemDanh { get; set; } = new List<DiemDanh>();
    }
}
