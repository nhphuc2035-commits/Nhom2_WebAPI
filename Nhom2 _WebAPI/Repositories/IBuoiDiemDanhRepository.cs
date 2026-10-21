using Nhom2__WebAPI.Models;

namespace Nhom2__WebAPI.Repositories
{
    public interface IBuoiDiemDanhRepository
    {
        Task<IEnumerable<BuoiDiemDanh>> GetBuoiDiemDanhsAsync();
        Task<BuoiDiemDanh?> GetBuoiDiemDanhByIdAsync(int id);
        Task<IEnumerable<BuoiDiemDanh>> GetBuoiDiemDanhByLopHocPhanAsync(int lopHocPhanId);
        Task AddBuoiDiemDanhAsync(BuoiDiemDanh buoi);
        Task UpdateBuoiDiemDanhAsync(BuoiDiemDanh buoi);
        Task DeleteBuoiDiemDanhAsync(int id);
        Task<bool> ToggleTrangThaiAsync(int id);
    }
}
