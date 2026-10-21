using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nhom2__WebAPI.Models;
using Nhom2__WebAPI.Models.DTOs;

namespace Nhom2__WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThongKeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ThongKeController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Tính năng mới độc quyền: Quét và Cảnh báo nguy cơ cấm thi cho sinh viên theo Quy chế đào tạo tín chỉ (Vắng >= 20%)
        /// </summary>
        [HttpGet("canh-bao-cam-thi/{lopHocPhanId}")]
        public async Task<IActionResult> GetCanhBaoCamThi(int lopHocPhanId)
        {
            try
            {
                var lhp = await _context.LopHocPhans
                    .Include(l => l.MonHoc)
                    .Include(l => l.BuoiDiemDanhs)
                    .FirstOrDefaultAsync(l => l.Id == lopHocPhanId);

                if (lhp == null)
                    return NotFound(new { Message = $"Không tìm thấy lớp học phần có Id {lopHocPhanId}" });

                // Danh sách sinh viên thuộc lớp
                var sinhViens = await _context.SinhVienLopHocPhans
                    .Where(x => x.LopHocPhanId == lopHocPhanId)
                    .Include(x => x.SinhVien)
                    .Select(x => x.SinhVien!)
                    .ToListAsync();

                var buoiDaToChuc = lhp.BuoiDiemDanhs.Where(b => b.TrangThai == "DaKetThuc").ToList();
                int tongBuoiDaToChuc = buoiDaToChuc.Count;
                var buoiDaToChucIds = buoiDaToChuc.Select(b => b.Id).ToList();
                int tongSoBuoiQuyDinh = lhp.SoBuoi > 0 ? lhp.SoBuoi : 9;

                var allDiemDanhs = await _context.DiemDanhs
                    .Where(d => buoiDaToChucIds.Contains(d.BuoiDiemDanhId))
                    .ToListAsync();

                var danhSachBaoCao = new List<SinhVienCanhBaoDto>();

                foreach (var sv in sinhViens)
                {
                    var svDiemDanhs = allDiemDanhs.Where(d => d.SinhVienId == sv.Id).ToList();

                    int soBuoiDungGio = svDiemDanhs.Count(d => d.HopLe && d.TrangThai == "CoMat");
                    int soBuoiDiTre = svDiemDanhs.Count(d => d.HopLe && d.TrangThai == "DiTre");
                    int soBuoiCoMat = soBuoiDungGio + soBuoiDiTre;
                    int soBuoiCoPhep = svDiemDanhs.Count(d => d.TrangThai == "VangCoPhep");
                    
                    // Số buổi vắng không phép = Tổng buổi đã kết thúc - Số buổi có mặt/đi trễ - Có phép
                    int soBuoiVangKhongPhep = tongBuoiDaToChuc - soBuoiCoMat - soBuoiCoPhep;
                    if (soBuoiVangKhongPhep < 0) soBuoiVangKhongPhep = 0;

                    // Tỷ lệ vắng tính theo tổng số buổi học phần quy định (9 buổi)
                    double tiLeVang = Math.Round(((double)soBuoiVangKhongPhep / tongSoBuoiQuyDinh) * 100, 1);

                    string mucDo = "BinhThuong";
                    string khuyenNghi = "Chuyên cần xuất sắc: 100% đúng giờ (trước 07:30 ca sáng / 12:30 ca chiều).";

                    // Quy chế tín chỉ HUTECH: Vắng >= 20% (từ 2 buổi trở lên) thì bị cấm thi
                    if (tiLeVang >= 20.0)
                    {
                        mucDo = "CamThi";
                        khuyenNghi = $"CẤM THI CHÍNH THỨC! Đã vắng {soBuoiVangKhongPhep}/{tongSoBuoiQuyDinh} buổi ({tiLeVang}% >= 20%). Không đủ điều kiện dự thi cuối kỳ.";
                    }
                    else if (soBuoiVangKhongPhep >= 1)
                    {
                        mucDo = "CanhBao_NguyCo";
                        khuyenNghi = $"CẢNH BÁO NGUY CƠ! Đã vắng {soBuoiVangKhongPhep}/{tongSoBuoiQuyDinh} buổi ({tiLeVang}%). Tối đa chỉ được vắng 1 buổi trong học phần 9 buổi, vắng thêm 1 buổi nữa sẽ bị CẤM THI.";
                    }
                    else if (soBuoiDiTre > 0)
                    {
                        mucDo = "DiTre";
                        khuyenNghi = $"Cần lưu ý: Có {soBuoiDiTre} buổi đi trễ (sau 07:30 ca sáng / 12:30 ca chiều). Hãy đi học đúng giờ để bảo đảm điểm rèn luyện.";
                    }

                    danhSachBaoCao.Add(new SinhVienCanhBaoDto
                    {
                        SinhVienId = sv.Id,
                        MaSV = sv.MaSV,
                        HoTen = sv.HoTen,
                        Lop = sv.Lop ?? string.Empty,
                        TongSoBuoi = tongSoBuoiQuyDinh,
                        SoBuoiCoMat = soBuoiCoMat,
                        SoBuoiDiTre = soBuoiDiTre,
                        SoBuoiVang = soBuoiVangKhongPhep,
                        SoBuoiVangCoPhep = soBuoiCoPhep,
                        TiLeVangPhanTram = tiLeVang,
                        MucDoCanhBao = mucDo,
                        KhuyenNghi = khuyenNghi
                    });
                }

                var result = new BaoCaoLopHocPhanDto
                {
                    LopHocPhanId = lhp.Id,
                    MaLopHocPhan = lhp.MaLopHocPhan,
                    TenMonHoc = lhp.MonHoc?.TenMon ?? string.Empty,
                    TongSinhVien = sinhViens.Count,
                    TongBuoiDaDiemDanh = tongBuoiDaToChuc,
                    SoSinhVienNguyCoCamThi = danhSachBaoCao.Count(x => x.MucDoCanhBao == "CanhBao_NguyCo"),
                    SoSinhVienBiCamThi = danhSachBaoCao.Count(x => x.MucDoCanhBao == "CamThi"),
                    DanhSachSinhVien = danhSachBaoCao.OrderByDescending(x => x.TiLeVangPhanTram).ToList()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi khi thống kê cảnh báo cấm thi", Error = ex.Message });
            }
        }

        /// <summary>
        /// Dashboard tổng quan dành cho Admin / Quản trị viên
        /// </summary>
        [HttpGet("tong-quan")]
        public async Task<IActionResult> GetDashboardTongQuan()
        {
            try
            {
                var today = DateTime.Today;
                var tongMon = await _context.MonHocs.CountAsync();
                var tongLhp = await _context.LopHocPhans.CountAsync();
                var tongSv = await _context.SinhViens.CountAsync();

                var buoiHomNay = await _context.BuoiDiemDanhs
                    .Where(b => b.NgayDiemDanh.Date == today)
                    .CountAsync();

                var luotDiemDanhHomNay = await _context.DiemDanhs
                    .Where(d => d.ThoiGianDiemDanh.Date == today)
                    .CountAsync();

                return Ok(new DashboardOverviewDto
                {
                    TongSoMonHoc = tongMon,
                    TongSoLopHocPhan = tongLhp,
                    TongSoSinhVien = tongSv,
                    TongSoBuoiDiemDanhHomNay = buoiHomNay,
                    TongSoLuotDiemDanhHomNay = luotDiemDanhHomNay,
                    SoSinhVienNguyCoCamThiToanTruong = 0
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        /// <summary>
        /// Thống kê chi tiết chuyên cần của một sinh viên qua các môn
        /// </summary>
        [HttpGet("sinh-vien/{sinhVienId}")]
        public async Task<IActionResult> GetThongKeSinhVien(int sinhVienId)
        {
            try
            {
                var sv = await _context.SinhViens.FindAsync(sinhVienId);
                if (sv == null) return NotFound(new { Message = "Không tìm thấy sinh viên" });

                var lhpList = await _context.SinhVienLopHocPhans
                    .Where(x => x.SinhVienId == sinhVienId)
                    .Include(x => x.LopHocPhan)
                        .ThenInclude(l => l!.MonHoc)
                    .Select(x => x.LopHocPhan!)
                    .ToListAsync();

                var records = await _context.DiemDanhs
                    .Where(d => d.SinhVienId == sinhVienId)
                    .Include(d => d.BuoiDiemDanh)
                    .ToListAsync();

                var result = lhpList.Select(lhp =>
                {
                    var buoiIds = _context.BuoiDiemDanhs.Where(b => b.LopHocPhanId == lhp.Id).Select(b => b.Id).ToList();
                    var svBuoiRecords = records.Where(r => buoiIds.Contains(r.BuoiDiemDanhId)).ToList();
                    int coMat = svBuoiRecords.Count(r => r.HopLe && (r.TrangThai == "CoMat" || r.TrangThai == "DiTre"));
                    int vangCoPhep = svBuoiRecords.Count(r => r.TrangThai == "VangCoPhep");
                    int vangKhongPhep = buoiIds.Count - coMat - vangCoPhep;
                    if (vangKhongPhep < 0) vangKhongPhep = 0;

                    return new
                    {
                        LopHocPhanId = lhp.Id,
                        lhp.MaLopHocPhan,
                        TenMon = lhp.MonHoc?.TenMon,
                        TongBuoiDaMo = buoiIds.Count,
                        SoBuoiCoMat = coMat,
                        SoBuoiVangCoPhep = vangCoPhep,
                        SoBuoiVangKhongPhep = vangKhongPhep,
                        TiLeVang = lhp.SoBuoi > 0 ? Math.Round(((double)vangKhongPhep / lhp.SoBuoi) * 100, 1) : 0
                    };
                });

                return Ok(new
                {
                    SinhVien = new { sv.Id, sv.MaSV, sv.HoTen, sv.Lop },
                    CacMonHoc = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }
    }
}
