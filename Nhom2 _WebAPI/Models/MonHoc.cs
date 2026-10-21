using System.ComponentModel.DataAnnotations;

namespace Nhom2__WebAPI.Models
{
    public class MonHoc
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string MaMon { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string TenMon { get; set; } = string.Empty;

        public int SoTinChi { get; set; } = 3;

        [MaxLength(500)]
        public string? MoTa { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<LopHocPhan> LopHocPhans { get; set; } = new List<LopHocPhan>();
    }
}
