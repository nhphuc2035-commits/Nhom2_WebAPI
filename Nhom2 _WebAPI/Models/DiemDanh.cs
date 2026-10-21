using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nhom2__WebAPI.Models
{
    public class DiemDanh
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BuoiDiemDanhId { get; set; }

        [ForeignKey("BuoiDiemDanhId")]
        public BuoiDiemDanh? BuoiDiemDanh { get; set; }

        [Required]
        public int SinhVienId { get; set; }

        [ForeignKey("SinhVienId")]
        public SinhVien? SinhVien { get; set; }

        public DateTime ThoiGianDiemDanh { get; set; } = DateTime.UtcNow;

        // PhuongThuc: "QR_Code", "GPS", "VanTay", "ThuCong" (Yêu cầu 5b: Vị trí, 5e: Vân tay)
        [MaxLength(30)]
        public string PhuongThuc { get; set; } = "QR_Code";

        // Vị trí sinh viên khi điểm danh
        public double? ViTriViDo { get; set; }
        public double? ViTriKinhDo { get; set; }
        public double? KhoangCachMet { get; set; }

        // TrangThai: "CoMat", "DiTre", "VangCoPhep", "VangKhongPhep"
        [MaxLength(30)]
        public string TrangThai { get; set; } = "CoMat";

        public bool HopLe { get; set; } = true;

        [MaxLength(255)]
        public string? LyDoKhongHopLe { get; set; }

        [MaxLength(500)]
        public string? GhiChu { get; set; }

        // Ảnh chụp / minh chứng (Yêu cầu 5f: Dịch ảnh chụp / ảnh minh chứng)
        [MaxLength(500)]
        public string? AnhMinhChung { get; set; }
    }
}
