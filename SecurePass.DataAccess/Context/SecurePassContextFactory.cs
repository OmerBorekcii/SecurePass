using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SecurePass.DataAccess.Context
{
    // Design-Time DbContext Factory: 
    // Entity Framework Core'un Migration (Veritabanı güncelleme) komutlarını ("dotnet ef migrations add")
    // çalıştırabilmesi için DbContext'i nasıl oluşturacağını anlattığımız fabrika sınıfıdır.
    public class SecurePassContextFactory : IDesignTimeDbContextFactory<SecurePassContext>
    {
        // Komut satırından EF araçları çalıştırıldığında bu metot tetiklenir.
        public SecurePassContext CreateDbContext(string[] args)
        {
            // Veritabanı bağlantı ayarlarını yapılandırmak için builder oluşturuyoruz.
            var optionsBuilder = new DbContextOptionsBuilder<SecurePassContext>();
            
            // Gerçek projelerde bu ConnectionString appsettings.json'dan okunur, 
            // ancak tasarım zamanı (design-time) için buraya sabit (hardcoded) yazabiliriz.
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=SecurePassDb;Trusted_Connection=True;MultipleActiveResultSets=true");

            // Yapılandırılmış ayarlarla yeni bir Context örneği döndürüyoruz.
            return new SecurePassContext(optionsBuilder.Options);
        }
    }
}