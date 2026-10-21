using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nhom2__WebAPI.Models
{
    public class SinhVienLopHocPhan
    {
        [Key]
        public int Id { get; set; }

        public int SinhVienId { get; set; }
        [ForeignKey("SinhVienId")]
        public SinhVien? SinhVien { get; set; }

        public int LopHocPhanId { get; set; }
        [ForeignKey("LopHocPhanId")]
        public LopHocPhan? LopHocPhan { get; set; }

        public DateTime NgayDangKy { get; set; } = DateTime.UtcNow;
    }
}
