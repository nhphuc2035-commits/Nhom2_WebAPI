using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Nhom2__WebAPI.Models
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<SinhVien> SinhViens { get; set; }
        public DbSet<MonHoc> MonHocs { get; set; }
        public DbSet<LopHocPhan> LopHocPhans { get; set; }
        public DbSet<SinhVienLopHocPhan> SinhVienLopHocPhans { get; set; }
        public DbSet<BuoiDiemDanh> BuoiDiemDanhs { get; set; }
        public DbSet<DiemDanh> DiemDanhs { get; set; }
        public DbSet<DonNghiPhep> DonNghiPheps { get; set; }
        public DbSet<OtpRecord> OtpRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().Property(u => u.Initials).HasMaxLength(10);
            builder.HasDefaultSchema("identity");

            // Quan he SinhVienLopHocPhan
            builder.Entity<SinhVienLopHocPhan>()
                .HasOne(svlhp => svlhp.SinhVien)
                .WithMany(sv => sv.SinhVienLopHocPhans)
                .HasForeignKey(svlhp => svlhp.SinhVienId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<SinhVienLopHocPhan>()
                .HasOne(svlhp => svlhp.LopHocPhan)
                .WithMany(lhp => lhp.SinhVienLopHocPhans)
                .HasForeignKey(svlhp => svlhp.LopHocPhanId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan he DiemDanh
            builder.Entity<DiemDanh>()
                .HasOne(dd => dd.BuoiDiemDanh)
                .WithMany(b => b.DanhSachDiemDanh)
                .HasForeignKey(dd => dd.BuoiDiemDanhId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<DiemDanh>()
                .HasOne(dd => dd.SinhVien)
                .WithMany(sv => sv.DanhSachDiemDanh)
                .HasForeignKey(dd => dd.SinhVienId)
                .OnDelete(DeleteBehavior.Restrict);

            // Tranh trung lap diem danh 1 sinh vien trong 1 buoi
            builder.Entity<DiemDanh>()
                .HasIndex(dd => new { dd.BuoiDiemDanhId, dd.SinhVienId })
                .IsUnique();
        }
    }
}
