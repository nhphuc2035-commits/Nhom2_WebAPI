using Microsoft.EntityFrameworkCore;
using Nhom2__WebAPI.Models;

namespace Nhom2__WebAPI.Repositories
{
    public class MonHocRepository : IMonHocRepository
    {
        private readonly ApplicationDbContext _context;

        public MonHocRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MonHoc>> GetMonHocsAsync()
        {
            return await _context.MonHocs.ToListAsync();
        }

        public async Task<MonHoc?> GetMonHocByIdAsync(int id)
        {
            return await _context.MonHocs.FindAsync(id);
        }

        public async Task<MonHoc?> GetMonHocByMaMonAsync(string maMon)
        {
            return await _context.MonHocs.FirstOrDefaultAsync(m => m.MaMon == maMon);
        }

        public async Task AddMonHocAsync(MonHoc monHoc)
        {
            _context.MonHocs.Add(monHoc);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMonHocAsync(MonHoc monHoc)
        {
            var existing = await _context.MonHocs.FindAsync(monHoc.Id);
            if (existing != null)
            {
                existing.MaMon = monHoc.MaMon;
                existing.TenMon = monHoc.TenMon;
                existing.SoTinChi = monHoc.SoTinChi;
                existing.MoTa = monHoc.MoTa;
                await _context.SaveChangesAsync();
            }
            else
            {
                _context.Entry(monHoc).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteMonHocAsync(int id)
        {
            var monHoc = await _context.MonHocs.FindAsync(id);
            if (monHoc != null)
            {
                _context.MonHocs.Remove(monHoc);
                await _context.SaveChangesAsync();
            }
        }
    }
}
