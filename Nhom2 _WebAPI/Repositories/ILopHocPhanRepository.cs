using Nhom2__WebAPI.Models;

namespace Nhom2__WebAPI.Repositories
{
    public interface ILopHocPhanRepository
    {
        Task<IEnumerable<LopHocPhan>> GetLopHocPhansAsync();
        Task<LopHocPhan?> GetLopHocPhanByIdAsync(int id);
        Task AddLopHocPhanAsync(LopHocPhan lopHocPhan);
        Task UpdateLopHocPhanAsync(LopHocPhan lopHocPhan);
        Task DeleteLopHocPhanAsync(int id);
        Task<IEnumerable<SinhVien>> GetSinhViensInLopHocPhanAsync(int lopHocPhanId);
        Task<bool> AddSinhVienToLopAsync(int lopHocPhanId, int sinhVienId);
        Task<bool> RemoveSinhVienFromLopAsync(int lopHocPhanId, int sinhVienId);
    }
}
