using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nhom2__WebAPI.Models
{
    public class BuoiDiemDanh
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int LopHocPhanId { get; set; }

        [ForeignKey("LopHocPhanId")]
        public LopHocPhan? LopHocPhan { get; set; }

        public int SoThuTuBuoi { get; set; } = 1;

        public DateTime NgayDiemDanh { get; set; } = DateTime.Today;

        public TimeSpan GioBatDau { get; set; } = new TimeSpan(7, 0, 0);
        public TimeSpan GioKetThuc { get; set; } = new TimeSpan(11, 30, 0);

        // QR Code token string
        [MaxLength(255)]
        public string? MaQRCode { get; set; }

        // Vị trí GPS của phòng học (Yêu cầu 5b: Chia sẻ vị trí)
        public double? ViTriViDo { get; set; } // Latitude
        public double? ViTriKinhDo { get; set; } // Longitude
        public double BanKinhChoPhepMet { get; set; } = 100; // Khoảng cách GPS tối đa tính bằng mét

        // Trạng thái: "DangMo", "DaDong"
        [MaxLength(20)]
        public string TrangThai { get; set; } = "DangMo";

        [MaxLength(255)]
        public string? GhiChu { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<DiemDanh> DanhSachDiemDanh { get; set; } = new List<DiemDanh>();
    }
}
