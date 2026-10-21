using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nhom2__WebAPI.Models
{
    public class DonNghiPhep
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SinhVienId { get; set; }

        [ForeignKey("SinhVienId")]
        public SinhVien? SinhVien { get; set; }

        [Required]
        public int LopHocPhanId { get; set; }

        [ForeignKey("LopHocPhanId")]
        public LopHocPhan? LopHocPhan { get; set; }

        public DateTime NgayNghi { get; set; }

        [Required]
        [MaxLength(500)]
        public string LyDo { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? MinhChungUrl { get; set; }

        // "ChoDuyet", "DaDuyet", "TuChoi"
        [MaxLength(30)]
        public string TrangThai { get; set; } = "ChoDuyet";

        public DateTime? NgayGui { get; set; } = DateTime.UtcNow;

        [MaxLength(255)]
        public string? PhanHoiGiaoVien { get; set; }
    }
}
