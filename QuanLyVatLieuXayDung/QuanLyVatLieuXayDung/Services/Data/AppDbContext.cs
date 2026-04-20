using Microsoft.EntityFrameworkCore;
using QuanLyVatLieuXayDung.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyVatLieuXayDung.Services.Data
{
    public class AppDbContext: DbContext
    {
        public DbSet<VatLieu> VatLieus { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Thay đổi Server Name cho đúng với máy
            // Dùng để kết nối DB
            optionsBuilder.UseSqlServer("Server=DESKTOP-HP165L4\\SQLEXPRESS;Database=QuanLyVatLieuDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }
}
