using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nhom2__WebAPI.Models
{
    public class LopHocPhan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string MaLopHocPhan { get; set; } = string.Empty;

        [Required]
        public int MonHocId { get; set; }

        [ForeignKey("MonHocId")]
        public MonHoc? MonHoc { get; set; }

        [MaxLength(100)]
        public string? GiangVien { get; set; }

        [MaxLength(50)]
        public string? PhongHoc { get; set; }

        [MaxLength(20)]
        public string? HocKy { get; set; }

        [MaxLength(20)]
        public string? NamHoc { get; set; }

        public int SoBuoi { get; set; } = 15;

        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<SinhVienLopHocPhan> SinhVienLopHocPhans { get; set; } = new List<SinhVienLopHocPhan>();

        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<BuoiDiemDanh> BuoiDiemDanhs { get; set; } = new List<BuoiDiemDanh>();

        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<DonNghiPhep> DonNghiPheps { get; set; } = new List<DonNghiPhep>();
    }
}
