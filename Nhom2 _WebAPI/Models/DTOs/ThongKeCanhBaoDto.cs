namespace Nhom2__WebAPI.Models.DTOs
{
    public class SinhVienCanhBaoDto
    {
        public int SinhVienId { get; set; }
        public string MaSV { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public string Lop { get; set; } = string.Empty;
        public int TongSoBuoi { get; set; }
        public int SoBuoiCoMat { get; set; }
        public int SoBuoiDiTre { get; set; }
        public int SoBuoiVang { get; set; }
        public int SoBuoiVangCoPhep { get; set; }
        public double TiLeVangPhanTram { get; set; }
        public string MucDoCanhBao { get; set; } = "BinhThuong"; // "BinhThuong", "CanhBao_NguyCo", "CamThi"
        public string KhuyenNghi { get; set; } = string.Empty;
    }

    public class BaoCaoLopHocPhanDto
    {
        public int LopHocPhanId { get; set; }
        public string MaLopHocPhan { get; set; } = string.Empty;
        public string TenMonHoc { get; set; } = string.Empty;
        public int TongSinhVien { get; set; }
        public int TongBuoiDaDiemDanh { get; set; }
        public int SoSinhVienNguyCoCamThi { get; set; }
        public int SoSinhVienBiCamThi { get; set; }
        public List<SinhVienCanhBaoDto> DanhSachSinhVien { get; set; } = new List<SinhVienCanhBaoDto>();
    }

    public class DashboardOverviewDto
    {
        public int TongSoMonHoc { get; set; }
        public int TongSoLopHocPhan { get; set; }
        public int TongSoSinhVien { get; set; }
        public int TongSoBuoiDiemDanhHomNay { get; set; }
        public int TongSoLuotDiemDanhHomNay { get; set; }
        public int SoSinhVienNguyCoCamThiToanTruong { get; set; }
    }
}
