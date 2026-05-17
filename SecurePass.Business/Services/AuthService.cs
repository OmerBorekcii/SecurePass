using System.Collections.Generic;
using System.Linq;
using SecurePass.Core.Entities;
using SecurePass.DataAccess.Context;

namespace SecurePass.Business.Services
{
    // İş Katmanı (Business Layer): Veritabanı ile Arayüz arasındaki mantıksal işlemleri yönetir.
    public class AuthService : IAuthService
    {
        private readonly SecurePassContext _context;

        // Dependency Injection (DI) üzerinden DbContext'i içeri alıyoruz.
        public AuthService(SecurePassContext context)
        {
            _context = context;
        }

        // Login İşlemi: Veritabanında kullanıcı adı, şifre ve aktiflik durumuna göre arama yapar.
        public User? Authenticate(string username, string password)
        {
            // FirstOrDefault: Kayıt bulursa ilkini, bulamazsa NULL döndürür.
            return _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password && u.IsActive);
        }

        // Personel Listeleme: Sistemdeki tüm kayıtlı kullanıcıları getirir.
        public List<User> GetAllUsers() => _context.Users.ToList();

        // Yeni Personel Ekleme
        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges(); // Değişiklikleri SQL'e yansıt.
        }

        // Personel Silme
        public void DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            // Güvenlik Kuralı: ID'si 1 olan ana Admin hesabı asla silinemez.
            if (user != null && user.Id != 1) 
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }
    }
}