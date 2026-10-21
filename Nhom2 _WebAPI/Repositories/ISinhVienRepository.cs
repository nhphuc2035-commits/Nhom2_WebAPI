using Nhom2__WebAPI.Models;

namespace Nhom2__WebAPI.Repositories
{
    public interface ISinhVienRepository
    {
        Task<IEnumerable<SinhVien>> GetSinhViensAsync();
        Task<SinhVien?> GetSinhVienByIdAsync(int id);
        Task<SinhVien?> GetSinhVienByMaSVAsync(string maSv);
        Task<SinhVien?> GetSinhVienByUserIdAsync(string userId);
        Task AddSinhVienAsync(SinhVien sinhVien);
        Task UpdateSinhVienAsync(SinhVien sinhVien);
        Task DeleteSinhVienAsync(int id);
    }
}
