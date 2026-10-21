using Microsoft.EntityFrameworkCore;
using Nhom2__WebAPI.Models;

namespace Nhom2__WebAPI.Repositories
{
    public class BuoiDiemDanhRepository : IBuoiDiemDanhRepository
    {
        private readonly ApplicationDbContext _context;

        public BuoiDiemDanhRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BuoiDiemDanh>> GetBuoiDiemDanhsAsync()
        {
            return await _context.BuoiDiemDanhs
                .Include(b => b.LopHocPhan)
                    .ThenInclude(l => l!.MonHoc)
                .Include(b => b.DanhSachDiemDanh)
                .OrderByDescending(b => b.NgayDiemDanh)
                .ToListAsync();
        }

        public async Task<BuoiDiemDanh?> GetBuoiDiemDanhByIdAsync(int id)
        {
            return await _context.BuoiDiemDanhs
                .Include(b => b.LopHocPhan)
                    .ThenInclude(l => l!.MonHoc)
                .Include(b => b.DanhSachDiemDanh)
                    .ThenInclude(dd => dd.SinhVien)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<BuoiDiemDanh>> GetBuoiDiemDanhByLopHocPhanAsync(int lopHocPhanId)
        {
            return await _context.BuoiDiemDanhs
                .Where(b => b.LopHocPhanId == lopHocPhanId)
                .Include(b => b.DanhSachDiemDanh)
                .OrderBy(b => b.SoThuTuBuoi)
                .ToListAsync();
        }

        public async Task AddBuoiDiemDanhAsync(BuoiDiemDanh buoi)
        {
            if (string.IsNullOrEmpty(buoi.MaQRCode))
            {
                buoi.MaQRCode = $"QR_LHP{buoi.LopHocPhanId}_B{buoi.SoThuTuBuoi}_{Guid.NewGuid().ToString("N")[..8]}";
            }
            _context.BuoiDiemDanhs.Add(buoi);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBuoiDiemDanhAsync(BuoiDiemDanh buoi)
        {
            var existing = await _context.BuoiDiemDanhs.FindAsync(buoi.Id);
            if (existing != null)
            {
                existing.LopHocPhanId = buoi.LopHocPhanId;
                existing.SoThuTuBuoi = buoi.SoThuTuBuoi;
                existing.NgayDiemDanh = buoi.NgayDiemDanh;
                existing.GioBatDau = buoi.GioBatDau;
                existing.GioKetThuc = buoi.GioKetThuc;
                existing.MaQRCode = buoi.MaQRCode;
                existing.ViTriViDo = buoi.ViTriViDo;
                existing.ViTriKinhDo = buoi.ViTriKinhDo;
                existing.BanKinhChoPhepMet = buoi.BanKinhChoPhepMet;
                existing.TrangThai = buoi.TrangThai;
                existing.GhiChu = buoi.GhiChu;
                await _context.SaveChangesAsync();
            }
            else
            {
                _context.Entry(buoi).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteBuoiDiemDanhAsync(int id)
        {
            var buoi = await _context.BuoiDiemDanhs.FindAsync(id);
            if (buoi != null)
            {
                _context.BuoiDiemDanhs.Remove(buoi);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ToggleTrangThaiAsync(int id)
        {
            var buoi = await _context.BuoiDiemDanhs.FindAsync(id);
            if (buoi == null) return false;

            buoi.TrangThai = buoi.TrangThai == "DangMo" ? "DaDong" : "DangMo";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
