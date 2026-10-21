using System.ComponentModel.DataAnnotations;

namespace Nhom2__WebAPI.Models.DTOs
{
    public class CheckInRequestDto
    {
        [Required]
        public int BuoiDiemDanhId { get; set; }

        [Required]
        public string MaSV { get; set; } = string.Empty;

        // Mã QR quét được (nếu điểm danh qua QR)
        public string? MaQRCode { get; set; }

        // Tọa độ GPS sinh viên gửi lên (Yêu cầu 5b: Chia sẻ vị trí)
        public double? ViTriViDo { get; set; }
        public double? ViTriKinhDo { get; set; }

        // Phương thức: "QR_Code", "GPS", "VanTay", "KhuonMat" (Yêu cầu 5e: Nhận diện vân tay)
        public string PhuongThuc { get; set; } = "QR_Code";

        // Ảnh chụp minh chứng / khuôn mặt (Yêu cầu 5f)
        public string? AnhMinhChung { get; set; }

        public string? GhiChu { get; set; }
    }

    public class CheckInResponseDto
    {
        public bool ThanhCong { get; set; }
        public string ThongBao { get; set; } = string.Empty;
        public string? TrangThai { get; set; }
        public double? KhoangCachMet { get; set; }
        public DateTime ThoiGian { get; set; }
    }
}
