using Nhom2__WebAPI.Models;

namespace Nhom2__WebAPI.Repositories
{
    public interface IMonHocRepository
    {
        Task<IEnumerable<MonHoc>> GetMonHocsAsync();
        Task<MonHoc?> GetMonHocByIdAsync(int id);
        Task<MonHoc?> GetMonHocByMaMonAsync(string maMon);
        Task AddMonHocAsync(MonHoc monHoc);
        Task UpdateMonHocAsync(MonHoc monHoc);
        Task DeleteMonHocAsync(int id);
    }
}
