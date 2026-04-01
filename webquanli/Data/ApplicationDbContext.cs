using Microsoft.EntityFrameworkCore;
using webquanli.Models;

namespace webquanli.Data
{

    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {

        }

        public DbSet<User> Users { get; set; }

        public DbSet<SinhVien> SinhViens { get; set; }

        public DbSet<GiangVien> GiangViens { get; set; }

        public DbSet<DeTai> DeTais { get; set; }

        public DbSet<DangKyDeTai> DangKyDeTais { get; set; }

        public DbSet<TienDo> TienDos { get; set; }

        public DbSet<BaoCao> BaoCaos { get; set; }

        public DbSet<DotDoAn> DotDoAns { get; set; }

        public DbSet<BoMon> BoMons { get; set; }

    }

}