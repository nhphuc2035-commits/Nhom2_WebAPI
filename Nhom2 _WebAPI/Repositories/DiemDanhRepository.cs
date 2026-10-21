using Microsoft.EntityFrameworkCore;
using Nhom2__WebAPI.Helpers;
using Nhom2__WebAPI.Models;
using Nhom2__WebAPI.Models.DTOs;

namespace Nhom2__WebAPI.Repositories
{
    public class DiemDanhRepository : IDiemDanhRepository
    {
        private readonly ApplicationDbContext _context;

        public DiemDanhRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DiemDanh>> GetDiemDanhsAsync()
        {
            return await _context.DiemDanhs
                .AsNoTracking()
                .Include(d => d.SinhVien)
                .Include(d => d.BuoiDiemDanh)
                    .ThenInclude(b => b!.LopHocPhan)
                .OrderByDescending(d => d.ThoiGianDiemDanh)
                .ToListAsync();
        }

        public async Task<DiemDanh?> GetDiemDanhByIdAsync(int id)
        {
            return await _context.DiemDanhs
                .AsNoTracking()
                .Include(d => d.SinhVien)
                .Include(d => d.BuoiDiemDanh)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<DiemDanh>> GetDiemDanhByBuoiAsync(int buoiDiemDanhId)
        {
            return await _context.DiemDanhs
                .AsNoTracking()
                .Where(d => d.BuoiDiemDanhId == buoiDiemDanhId)
                .Include(d => d.SinhVien)
                .OrderBy(d => d.ThoiGianDiemDanh)
                .ToListAsync();
        }

        public async Task<IEnumerable<DiemDanh>> GetDiemDanhBySinhVienAsync(int sinhVienId)
        {
            return await _context.DiemDanhs
                .AsNoTracking()
                .Where(d => d.SinhVienId == sinhVienId)
                .Include(d => d.BuoiDiemDanh)
                    .ThenInclude(b => b!.LopHocPhan)
                        .ThenInclude(l => l!.MonHoc)
                .OrderByDescending(d => d.ThoiGianDiemDanh)
                .ToListAsync();
        }

        public async Task<CheckInResponseDto> CheckInAsync(CheckInRequestDto request)
        {
            // 1. Kiểm tra buổi điểm danh
            var buoi = await _context.BuoiDiemDanhs
                .Include(b => b.LopHocPhan)
                .FirstOrDefaultAsync(b => b.Id == request.BuoiDiemDanhId);

            if (buoi == null)
            {
                return new CheckInResponseDto
                {
                    ThanhCong = false,
                    ThongBao = "Không tìm thấy buổi điểm danh này.",
                    ThoiGian = DateTime.UtcNow
                };
            }

            if (buoi.TrangThai != "DangMo")
            {
                return new CheckInResponseDto
                {
                    ThanhCong = false,
                    ThongBao = "Buổi điểm danh này hiện đang đóng hoặc đã kết thúc.",
                    ThoiGian = DateTime.UtcNow
                };
            }

            // 2. Kiểm tra sinh viên
            var sv = await _context.SinhViens.FirstOrDefaultAsync(s => s.MaSV == request.MaSV);
            if (sv == null)
            {
                return new CheckInResponseDto
                {
                    ThanhCong = false,
                    ThongBao = $"Không tìm thấy thông tin sinh viên với MSSV: {request.MaSV}",
                    ThoiGian = DateTime.UtcNow
                };
            }

            // Kiểm tra sinh viên có trong lớp học phần không
            var thuocLop = await _context.SinhVienLopHocPhans
                .AnyAsync(x => x.LopHocPhanId == buoi.LopHocPhanId && x.SinhVienId == sv.Id);
            if (!thuocLop)
            {
                return new CheckInResponseDto
                {
                    ThanhCong = false,
                    ThongBao = "Sinh viên không nằm trong danh sách của lớp học phần này.",
                    ThoiGian = DateTime.UtcNow
                };
            }

            // 3. Kiểm tra đã điểm danh chưa
            var daDiemDanh = await _context.DiemDanhs
                .FirstOrDefaultAsync(d => d.BuoiDiemDanhId == buoi.Id && d.SinhVienId == sv.Id);

            if (daDiemDanh != null)
            {
                return new CheckInResponseDto
                {
                    ThanhCong = false,
                    ThongBao = $"Bạn đã điểm danh buổi học này vào lúc {daDiemDanh.ThoiGianDiemDanh.ToLocalTime():HH:mm:ss}.",
                    TrangThai = daDiemDanh.TrangThai,
                    KhoangCachMet = daDiemDanh.KhoangCachMet,
                    ThoiGian = daDiemDanh.ThoiGianDiemDanh
                };
            }

            // 4. Kiểm tra mã QR (nếu phương thức điểm danh là QR_Code)
            if (request.PhuongThuc.Contains("QR", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(request.MaQRCode) || request.MaQRCode != buoi.MaQRCode)
                {
                    return new CheckInResponseDto
                    {
                        ThanhCong = false,
                        ThongBao = "Mã QR không đúng hoặc đã hết hạn.",
                        ThoiGian = DateTime.UtcNow
                    };
                }
            }

            // 5. Kiểm tra vị trí địa lý GPS (Yêu cầu 5b)
            double? distance = null;
            bool isLocationValid = true;
            string? invalidReason = null;

            if (buoi.ViTriViDo.HasValue && buoi.ViTriKinhDo.HasValue &&
                request.ViTriViDo.HasValue && request.ViTriKinhDo.HasValue)
            {
                distance = GeoHelper.CalculateDistanceInMeters(
                    buoi.ViTriViDo.Value, buoi.ViTriKinhDo.Value,
                    request.ViTriViDo.Value, request.ViTriKinhDo.Value
                );

                // Cho phép lấy vị trí thoải mái khi bán kính cho phép rộng (buổi thực hành)
                if (buoi.BanKinhChoPhepMet < 10000 && distance > buoi.BanKinhChoPhepMet)
                {
                    isLocationValid = false;
                    invalidReason = $"Vị trí của bạn cách phòng học {distance}m (cho phép tối đa {buoi.BanKinhChoPhepMet}m).";
                }
            }

            // 6. Xác định trạng thái theo quy định ca học:
            // - Ca sáng: 07:30 - 11:30 (Đúng giờ: <= 07:30; Qua 07:30: Đi trễ; Qua 11:30: Vắng không phép)
            // - Ca chiều: 12:30 - 16:30 (Đúng giờ: <= 12:30; Qua 12:30: Đi trễ; Qua 16:30: Vắng không phép)
            var nowTime = DateTime.Now.TimeOfDay;
            string trangThai = "CoMat";
            if (nowTime > buoi.GioKetThuc)
            {
                trangThai = "VangKhongPhep";
                isLocationValid = false;
                invalidReason = $"Đã quá giờ kết thúc ca học ({buoi.GioKetThuc:hh\\:mm}). Hệ thống ghi nhận vắng buổi học này.";
            }
            else if (nowTime > buoi.GioBatDau)
            {
                // Nếu buổi học mở rộng tới đêm (buổi kiểm tra thực hành), ghi nhận Có mặt
                trangThai = (buoi.GioKetThuc >= new TimeSpan(22, 0, 0)) ? "CoMat" : "DiTre";
            }

            var record = new DiemDanh
            {
                BuoiDiemDanhId = buoi.Id,
                SinhVienId = sv.Id,
                ThoiGianDiemDanh = DateTime.UtcNow,
                PhuongThuc = request.PhuongThuc,
                ViTriViDo = request.ViTriViDo,
                ViTriKinhDo = request.ViTriKinhDo,
                KhoangCachMet = distance,
                HopLe = isLocationValid,
                LyDoKhongHopLe = invalidReason,
                TrangThai = isLocationValid ? trangThai : "VangKhongPhep",
                GhiChu = request.GhiChu ?? (isLocationValid ? "Điểm danh hợp lệ" : invalidReason),
                AnhMinhChung = request.AnhMinhChung
            };

            _context.DiemDanhs.Add(record);
            await _context.SaveChangesAsync();

            return new CheckInResponseDto
            {
                ThanhCong = isLocationValid,
                ThongBao = isLocationValid ? $"Điểm danh thành công! Trạng thái: {trangThai}" : $"Điểm danh không hợp lệ: {invalidReason}",
                TrangThai = record.TrangThai,
                KhoangCachMet = distance,
                ThoiGian = record.ThoiGianDiemDanh
            };
        }

        public async Task UpdateDiemDanhAsync(DiemDanh diemDanh)
        {
            var existing = await _context.DiemDanhs.FindAsync(diemDanh.Id);
            if (existing != null)
            {
                existing.BuoiDiemDanhId = diemDanh.BuoiDiemDanhId;
                existing.SinhVienId = diemDanh.SinhVienId;
                existing.ThoiGianDiemDanh = diemDanh.ThoiGianDiemDanh;
                existing.PhuongThuc = diemDanh.PhuongThuc;
                existing.ViTriViDo = diemDanh.ViTriViDo;
                existing.ViTriKinhDo = diemDanh.ViTriKinhDo;
                existing.KhoangCachMet = diemDanh.KhoangCachMet;
                existing.TrangThai = diemDanh.TrangThai;
                existing.HopLe = diemDanh.HopLe;
                existing.LyDoKhongHopLe = diemDanh.LyDoKhongHopLe;
                existing.GhiChu = diemDanh.GhiChu;
                existing.AnhMinhChung = diemDanh.AnhMinhChung;
                await _context.SaveChangesAsync();
            }
            else
            {
                _context.Entry(diemDanh).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteDiemDanhAsync(int id)
        {
            var record = await _context.DiemDanhs.FindAsync(id);
            if (record != null)
            {
                _context.DiemDanhs.Remove(record);
                await _context.SaveChangesAsync();
            }
        }
    }
}
