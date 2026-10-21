using Microsoft.EntityFrameworkCore;
using Nhom2__WebAPI.Models;

namespace Nhom2__WebAPI.Repositories
{
    public class LopHocPhanRepository : ILopHocPhanRepository
    {
        private readonly ApplicationDbContext _context;

        public LopHocPhanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LopHocPhan>> GetLopHocPhansAsync()
        {
            return await _context.LopHocPhans
                .Include(l => l.MonHoc)
                .Include(l => l.SinhVienLopHocPhans)
                .ToListAsync();
        }

        public async Task<LopHocPhan?> GetLopHocPhanByIdAsync(int id)
        {
            return await _context.LopHocPhans
                .Include(l => l.MonHoc)
                .Include(l => l.SinhVienLopHocPhans)
                    .ThenInclude(svlhp => svlhp.SinhVien)
                .Include(l => l.BuoiDiemDanhs)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task AddLopHocPhanAsync(LopHocPhan lopHocPhan)
        {
            _context.LopHocPhans.Add(lopHocPhan);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLopHocPhanAsync(LopHocPhan lopHocPhan)
        {
            var existing = await _context.LopHocPhans.FindAsync(lopHocPhan.Id);
            if (existing != null)
            {
                existing.MaLopHocPhan = lopHocPhan.MaLopHocPhan;
                existing.MonHocId = lopHocPhan.MonHocId;
                existing.HocKy = lopHocPhan.HocKy;
                existing.NamHoc = lopHocPhan.NamHoc;
                existing.GiangVien = lopHocPhan.GiangVien;
                existing.PhongHoc = lopHocPhan.PhongHoc;
                existing.SoBuoi = lopHocPhan.SoBuoi;
                await _context.SaveChangesAsync();
            }
            else
            {
                _context.Entry(lopHocPhan).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteLopHocPhanAsync(int id)
        {
            var lopHocPhan = await _context.LopHocPhans.FindAsync(id);
            if (lopHocPhan != null)
            {
                _context.LopHocPhans.Remove(lopHocPhan);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<SinhVien>> GetSinhViensInLopHocPhanAsync(int lopHocPhanId)
        {
            return await _context.SinhVienLopHocPhans
                .Where(x => x.LopHocPhanId == lopHocPhanId)
                .Select(x => x.SinhVien!)
                .ToListAsync();
        }

        public async Task<bool> AddSinhVienToLopAsync(int lopHocPhanId, int sinhVienId)
        {
            var exists = await _context.SinhVienLopHocPhans
                .AnyAsync(x => x.LopHocPhanId == lopHocPhanId && x.SinhVienId == sinhVienId);
            if (exists) return false;

            _context.SinhVienLopHocPhans.Add(new SinhVienLopHocPhan
            {
                LopHocPhanId = lopHocPhanId,
                SinhVienId = sinhVienId,
                NgayDangKy = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveSinhVienFromLopAsync(int lopHocPhanId, int sinhVienId)
        {
            var item = await _context.SinhVienLopHocPhans
                .FirstOrDefaultAsync(x => x.LopHocPhanId == lopHocPhanId && x.SinhVienId == sinhVienId);
            if (item == null) return false;

            _context.SinhVienLopHocPhans.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
