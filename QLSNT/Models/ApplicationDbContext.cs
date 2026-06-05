using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QLSNT.Models;   // chỗ này chứa TinhCu, HuyenCu, XaCu

namespace QLSNT.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TinhCu> TinhCus { get; set; } = default!;
        public DbSet<HuyenCu> HuyenCus { get; set; } = default!;
        public DbSet<XaCu> XaCus { get; set; } = default!;

        // Tỉnh / xã mới
        public DbSet<TinhMoi> TinhMois { get; set; } = default!;
        public DbSet<XaMoi> XaMois { get; set; } = default!;

        // Sự kiện & lịch sử sáp nhập
        public DbSet<SuKienHanhChinh> SuKienHanhChinhs { get; set; } = default!;
        public DbSet<LichSuSapNhap> LichSuSapNhaps { get; set; } = default!;
        public DbSet<LssnTinh> LssnTinhs { get; set; } = default!;
        public DbSet<LssnXa> LssnXas { get; set; } = default!;

        // Người dân & địa chỉ
        public DbSet<NguoiDan> NguoiDans { get; set; } = default!;
        public DbSet<ThuongTru> ThuongTrus { get; set; } = default!;
        public DbSet<TamTru> TamTrus { get; set; } = default!;
        public DbSet<LichSuDiaChi> LichSuDiaChis { get; set; } = default!;

        // Danh mục
        public DbSet<DanToc> DanTocs { get; set; } = default!;
        public DbSet<TonGiao> TonGiaos { get; set; } = default!;
        public DbSet<QuanHeChuHo> QuanHeChuHos { get; set; } = default!;
        public DbSet<TrinhDoVanHoa> TrinhDoVanHoas { get; set; } = default!;
        public DbSet<NhatKyHoatDong> NhatKyHoatDongs { get; set; } = default!;
    }
}
