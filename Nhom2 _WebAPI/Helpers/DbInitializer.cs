using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nhom2__WebAPI.Models;

namespace Nhom2__WebAPI.Helpers
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // 1. Đảm bảo Database đã được tạo
            await context.Database.MigrateAsync();

            // 2. Tạo Roles
            var roles = new[] { "Admin", "SinhVien" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 3. Tạo tài khoản Admin mặc định
            var adminUser = await userManager.FindByNameAsync("admin");
            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = "admin",
                    Email = "admin@nhom2.edu.vn",
                    HoTen = "Quản trị viên Nhóm 2",
                    Initials = "AD",
                    MaSV = "ADMIN01"
                };
                var createResult = await userManager.CreateAsync(adminUser, "Admin@123");
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // 4. Tạo tài khoản Sinh viên mặc định
            var svUser = await userManager.FindByNameAsync("sinhvien1");
            if (svUser == null)
            {
                svUser = new User
                {
                    UserName = "sinhvien1",
                    Email = "sinhvien1@nhom2.edu.vn",
                    HoTen = "Nguyễn Hữu Phúc",
                    Initials = "HP",
                    MaSV = "2380601738"
                };
                var createSvResult = await userManager.CreateAsync(svUser, "Sinhvien@123");
                if (createSvResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(svUser, "SinhVien");
                }
            }

            // 5. Tạo dữ liệu Sinh viên trong bảng SinhVien
            var sv1 = await context.SinhViens.FirstOrDefaultAsync(s => s.MaSV == "2380601738");
            if (sv1 == null)
            {
                sv1 = new SinhVien
                {
                    MaSV = "2380601738",
                    HoTen = "Nguyễn Hữu Phúc",
                    Email = "sinhvien1@nhom2.edu.vn",
                    SoDienThoai = "0901234567",
                    Lop = "21DTH01",
                    Khoa = "Công nghệ thông tin",
                    UserId = svUser?.Id,
                    NgayTao = DateTime.UtcNow,
                    TrangThai = true
                };
                context.SinhViens.Add(sv1);

                // Thêm sinh viên mẫu thứ 2
                var sv2 = new SinhVien
                {
                    MaSV = "2380601823",
                    HoTen = "Hồng Thắng Quân",
                    Email = "quan@nhom2.edu.vn",
                    SoDienThoai = "0912345678",
                    Lop = "21DTH01",
                    Khoa = "Công nghệ thông tin",
                    NgayTao = DateTime.UtcNow,
                    TrangThai = true
                };
                context.SinhViens.Add(sv2);

                // Thêm sinh viên thứ 3: Nguyễn Hoài An
                var sv3 = new SinhVien
                {
                    MaSV = "2380600008",
                    HoTen = "Nguyễn Hoài An",
                    Email = "an@nhom2.edu.vn",
                    SoDienThoai = "0923456789",
                    Lop = "21DTH01",
                    Khoa = "Công nghệ thông tin",
                    NgayTao = DateTime.UtcNow,
                    TrangThai = true
                };
                context.SinhViens.Add(sv3);

                // Thêm sinh viên thứ 4: Tạ Thanh Mạnh
                var sv4 = new SinhVien
                {
                    MaSV = "2380604510",
                    HoTen = "Tạ Thanh Mạnh",
                    Email = "manh@nhom2.edu.vn",
                    SoDienThoai = "0934567890",
                    Lop = "21DTH01",
                    Khoa = "Công nghệ thông tin",
                    NgayTao = DateTime.UtcNow,
                    TrangThai = true
                };
                context.SinhViens.Add(sv4);

                await context.SaveChangesAsync();
            }

            // 6. Tạo Môn học mẫu
            if (!await context.MonHocs.AnyAsync())
            {
                var mh1 = new MonHoc
                {
                    MaMon = "LTDD01",
                    TenMon = "Lập trình thiết bị di động",
                    SoTinChi = 3,
                    MoTa = "Phát triển ứng dụng Android / Flutter và Web API kết nối cơ sở dữ liệu"
                };
                var mh2 = new MonHoc
                {
                    MaMon = "KTPM02",
                    TenMon = "Kiến trúc phần mềm",
                    SoTinChi = 3,
                    MoTa = "Kiến trúc hướng dịch vụ, Clean Architecture, RESTful API"
                };
                context.MonHocs.AddRange(mh1, mh2);
                await context.SaveChangesAsync();

                // 7. Tạo Lớp học phần mẫu
                var lhp1 = new LopHocPhan
                {
                    MaLopHocPhan = "24DTH01_LTDD",
                    MonHocId = mh1.Id,
                    GiangVien = "ThS. Nguyễn Hữu Trung",
                    PhongHoc = "A-04.05",
                    HocKy = "HK1",
                    NamHoc = "2026-2027",
                    SoBuoi = 15
                };
                context.LopHocPhans.Add(lhp1);
                await context.SaveChangesAsync();

                // Đăng ký sinh viên vào lớp
                var allSv = await context.SinhViens.ToListAsync();
                foreach (var s in allSv)
                {
                    context.SinhVienLopHocPhans.Add(new SinhVienLopHocPhan
                    {
                        LopHocPhanId = lhp1.Id,
                        SinhVienId = s.Id,
                        NgayDangKy = DateTime.UtcNow
                    });
                }
                await context.SaveChangesAsync();

                // 8. Tạo Buổi điểm danh mẫu (có tọa độ GPS HUTECH và mã QR)
                var buoi1 = new BuoiDiemDanh
                {
                    LopHocPhanId = lhp1.Id,
                    SoThuTuBuoi = 1,
                    NgayDiemDanh = DateTime.Today,
                    GioBatDau = new TimeSpan(7, 30, 0),
                    GioKetThuc = new TimeSpan(11, 30, 0),
                    MaQRCode = "QR_HUTECH_LTDD_BUOI01",
                    ViTriViDo = 10.8018, // Tọa độ HUTECH Bình Thạnh
                    ViTriKinhDo = 106.7142,
                    BanKinhChoPhepMet = 150.0,
                    TrangThai = "DangMo",
                    GhiChu = "Buổi 1: Giới thiệu môn học và cấu hình môi trường"
                };
                context.BuoiDiemDanhs.Add(buoi1);
                await context.SaveChangesAsync();
            }
        }
    }
}
