using System.ComponentModel.DataAnnotations;

namespace Nhom2__WebAPI.Models
{
    public class OtpRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string OtpCode { get; set; } = string.Empty;

        public DateTime ExpirationTime { get; set; }

        public bool IsUsed { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
