using Nhom2__WebAPI.Models;

namespace Nhom2__WebAPI.Repositories
{
    public interface IDonNghiPhepRepository
    {
        Task<IEnumerable<DonNghiPhep>> GetDonNghiPhepsAsync();
        Task<DonNghiPhep?> GetDonNghiPhepByIdAsync(int id);
        Task<IEnumerable<DonNghiPhep>> GetDonNghiPhepBySinhVienAsync(int sinhVienId);
        Task AddDonNghiPhepAsync(DonNghiPhep donNghiPhep);
        Task<bool> DuyetDonNghiPhepAsync(int id, string trangThai, string? phanHoi);
        Task DeleteDonNghiPhepAsync(int id);
    }
}
