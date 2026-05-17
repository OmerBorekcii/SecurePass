using Microsoft.EntityFrameworkCore;
using SecurePass.Core.Entities;

namespace SecurePass.DataAccess.Context
{
    // Bu sınıf, kodumuzla veritabanı arasındaki köprüdür (DbContext).
    // Veritabanı tablolarını ve ilk kurulum verilerini burada tanımlıyoruz.
    public class SecurePassContext : DbContext
    {
        public SecurePassContext(DbContextOptions<SecurePassContext> options) : base(options) { }

        // Tablo tanımlamalarımız
        public DbSet<User> Users { get; set; }
        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed Data: Sistem ilk kurulduğunda içinde varsayılan kullanıcılar olsun diye buraya ekliyoruz.
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", Password = "123", Role = "Admin" },
                new User { Id = 2, Username = "resepsiyon", Password = "123", Role = "Reception" }
            );
        }
    }
}