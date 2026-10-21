using Nhom2__WebAPI.Models;
using Nhom2__WebAPI.Models.DTOs;

namespace Nhom2__WebAPI.Repositories
{
    public interface IDiemDanhRepository
    {
        Task<IEnumerable<DiemDanh>> GetDiemDanhsAsync();
        Task<DiemDanh?> GetDiemDanhByIdAsync(int id);
        Task<IEnumerable<DiemDanh>> GetDiemDanhByBuoiAsync(int buoiDiemDanhId);
        Task<IEnumerable<DiemDanh>> GetDiemDanhBySinhVienAsync(int sinhVienId);
        Task<CheckInResponseDto> CheckInAsync(CheckInRequestDto request);
        Task UpdateDiemDanhAsync(DiemDanh diemDanh);
        Task DeleteDiemDanhAsync(int id);
    }
}
