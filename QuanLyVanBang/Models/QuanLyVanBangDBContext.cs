using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace QuanLyVanBang.Models
{
    public class QuanLyVanBangDBContext : DbContext
    {
        public QuanLyVanBangDBContext(DbContextOptions<QuanLyVanBangDBContext> options) : base(options) { }
        public DbSet<LoaiVanBan> LoaiVanBans { get; set; }
        public DbSet<DonVi> DonVis { get; set; }
        public DbSet<ChucVu> ChucVus { get; set; }
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<VanBanDi> VanBanDis { get; set; }
        public DbSet<VanBanDen> VanBanDens { get; set; }
        public DbSet<LichSuXuLy> LichSuXuLys { get; set; }
        public DbSet<ThongBao> ThongBaos { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<LoaiVanBan>().ToTable("LoaiVanBan");
            modelBuilder.Entity<DonVi>().ToTable("DonVi");
            modelBuilder.Entity<ChucVu>().ToTable("ChucVu");
            modelBuilder.Entity<NguoiDung>().ToTable("NguoiDung");
            modelBuilder.Entity<VanBanDi>().ToTable("VanBanDi");
            modelBuilder.Entity<VanBanDen>().ToTable("VanBanDen");
            modelBuilder.Entity<LichSuXuLy>().ToTable("LichSuXuLy");
            modelBuilder.Entity<ThongBao>().ToTable("ThongBao");
        }

    }
}
