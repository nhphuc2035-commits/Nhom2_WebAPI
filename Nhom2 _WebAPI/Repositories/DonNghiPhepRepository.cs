using Microsoft.EntityFrameworkCore;
using Nhom2__WebAPI.Models;

namespace Nhom2__WebAPI.Repositories
{
    public class DonNghiPhepRepository : IDonNghiPhepRepository
    {
        private readonly ApplicationDbContext _context;

        public DonNghiPhepRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DonNghiPhep>> GetDonNghiPhepsAsync()
        {
            return await _context.DonNghiPheps
                .Include(d => d.SinhVien)
                .Include(d => d.LopHocPhan)
                    .ThenInclude(l => l!.MonHoc)
                .OrderByDescending(d => d.NgayGui)
                .ToListAsync();
        }

        public async Task<DonNghiPhep?> GetDonNghiPhepByIdAsync(int id)
        {
            return await _context.DonNghiPheps
                .Include(d => d.SinhVien)
                .Include(d => d.LopHocPhan)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<DonNghiPhep>> GetDonNghiPhepBySinhVienAsync(int sinhVienId)
        {
            return await _context.DonNghiPheps
                .Where(d => d.SinhVienId == sinhVienId)
                .Include(d => d.LopHocPhan)
                    .ThenInclude(l => l!.MonHoc)
                .OrderByDescending(d => d.NgayGui)
                .ToListAsync();
        }

        public async Task AddDonNghiPhepAsync(DonNghiPhep donNghiPhep)
        {
            _context.DonNghiPheps.Add(donNghiPhep);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DuyetDonNghiPhepAsync(int id, string trangThai, string? phanHoi)
        {
            var don = await _context.DonNghiPheps.FindAsync(id);
            if (don == null) return false;

            don.TrangThai = trangThai; // "DaDuyet" hoac "TuChoi"
            don.PhanHoiGiaoVien = phanHoi;

            // Tìm buổi học tương ứng của lớp học phần vào ngày xin nghỉ
            var targetDate = don.NgayNghi.Date;
            var buoi = await _context.BuoiDiemDanhs
                .FirstOrDefaultAsync(b => b.LopHocPhanId == don.LopHocPhanId && b.NgayDiemDanh.Date == targetDate);

            // Nếu không tìm thấy theo đúng ngày, tìm buổi đang mở hoặc buổi gần nhất của lớp này
            if (buoi == null)
            {
                buoi = await _context.BuoiDiemDanhs
                    .FirstOrDefaultAsync(b => b.LopHocPhanId == don.LopHocPhanId && (b.TrangThai == "DangMo" || b.TrangThai == "Mo"));
            }

            if (buoi != null)
            {
                var diemDanh = await _context.DiemDanhs
                    .FirstOrDefaultAsync(d => d.BuoiDiemDanhId == buoi.Id && d.SinhVienId == don.SinhVienId);

                if (string.Equals(trangThai, "DaDuyet", StringComparison.OrdinalIgnoreCase))
                {
                    // Khi admin duyệt -> Chuyển sang VangCoPhep
                    if (diemDanh != null)
                    {
                        diemDanh.TrangThai = "VangCoPhep";
                        diemDanh.HopLe = true;
                        diemDanh.LyDoKhongHopLe = null;
                        diemDanh.GhiChu = $"Vắng có phép (Đơn nghỉ phép #{don.Id} đã được duyệt: {don.LyDo})";
                        if (!string.IsNullOrEmpty(don.MinhChungUrl))
                        {
                            diemDanh.AnhMinhChung = don.MinhChungUrl;
                        }
                    }
                    else
                    {
                        // Nếu chưa có bản ghi điểm danh -> Tạo mới bản ghi VangCoPhep
                        diemDanh = new DiemDanh
                        {
                            BuoiDiemDanhId = buoi.Id,
                            SinhVienId = don.SinhVienId,
                            ThoiGianDiemDanh = DateTime.UtcNow,
                            PhuongThuc = "DonNghiPhep",
                            TrangThai = "VangCoPhep",
                            HopLe = true,
                            GhiChu = $"Vắng có phép (Đơn nghỉ phép #{don.Id} đã được duyệt: {don.LyDo})",
                            AnhMinhChung = don.MinhChungUrl
                        };
                        _context.DiemDanhs.Add(diemDanh);
                    }
                }
                else if (string.Equals(trangThai, "TuChoi", StringComparison.OrdinalIgnoreCase))
                {
                    // Nếu admin từ chối và trước đó đã gán VangCoPhep -> Chuyển thành VangKhongPhep
                    if (diemDanh != null && diemDanh.TrangThai == "VangCoPhep")
                    {
                        diemDanh.TrangThai = "VangKhongPhep";
                        diemDanh.HopLe = false;
                        diemDanh.GhiChu = $"Vắng không phép (Đơn xin nghỉ bị từ chối: {phanHoi})";
                    }
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task DeleteDonNghiPhepAsync(int id)
        {
            var don = await _context.DonNghiPheps.FindAsync(id);
            if (don != null)
            {
                _context.DonNghiPheps.Remove(don);
                await _context.SaveChangesAsync();
            }
        }
    }
}
