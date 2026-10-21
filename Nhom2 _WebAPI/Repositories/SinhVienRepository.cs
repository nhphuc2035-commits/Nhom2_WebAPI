using Microsoft.EntityFrameworkCore;
using Nhom2__WebAPI.Models;

namespace Nhom2__WebAPI.Repositories
{
    public class SinhVienRepository : ISinhVienRepository
    {
        private readonly ApplicationDbContext _context;

        public SinhVienRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SinhVien>> GetSinhViensAsync()
        {
            return await _context.SinhViens.ToListAsync();
        }

        public async Task<SinhVien?> GetSinhVienByIdAsync(int id)
        {
            return await _context.SinhViens.FindAsync(id);
        }

        public async Task<SinhVien?> GetSinhVienByMaSVAsync(string maSv)
        {
            return await _context.SinhViens.FirstOrDefaultAsync(s => s.MaSV == maSv);
        }

        public async Task<SinhVien?> GetSinhVienByUserIdAsync(string userId)
        {
            return await _context.SinhViens.FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task AddSinhVienAsync(SinhVien sinhVien)
        {
            _context.SinhViens.Add(sinhVien);
            await _context.SaveChangesAsync();

            // Tự động ghi danh sinh viên mới vào tất cả lớp học phần đang hoạt động
            var activeLops = await _context.LopHocPhans.ToListAsync();
            foreach (var lop in activeLops)
            {
                bool daGhiDanh = await _context.SinhVienLopHocPhans
                    .AnyAsync(x => x.LopHocPhanId == lop.Id && x.SinhVienId == sinhVien.Id);
                if (!daGhiDanh)
                {
                    _context.SinhVienLopHocPhans.Add(new SinhVienLopHocPhan
                    {
                        SinhVienId = sinhVien.Id,
                        LopHocPhanId = lop.Id,
                        NgayDangKy = DateTime.UtcNow
                    });
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSinhVienAsync(SinhVien sinhVien)
        {
            var existing = await _context.SinhViens.FindAsync(sinhVien.Id);
            if (existing != null)
            {
                existing.MaSV = sinhVien.MaSV;
                existing.HoTen = sinhVien.HoTen;
                existing.Email = sinhVien.Email;
                existing.SoDienThoai = sinhVien.SoDienThoai;
                existing.Lop = sinhVien.Lop;
                existing.Khoa = sinhVien.Khoa;
                existing.AnhDaiDien = sinhVien.AnhDaiDien;
                existing.TrangThai = sinhVien.TrangThai;
                await _context.SaveChangesAsync();
            }
            else
            {
                _context.Entry(sinhVien).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteSinhVienAsync(int id)
        {
            var sinhVien = await _context.SinhViens.FindAsync(id);
            if (sinhVien != null)
            {
                _context.SinhViens.Remove(sinhVien);
                await _context.SaveChangesAsync();
            }
        }
    }
}
