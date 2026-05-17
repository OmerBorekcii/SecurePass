using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecurePass.Business.Services;
using SecurePass.Core.Entities;

namespace SecurePass.Web.Controllers
{
    // Personel Yönetimi sayfası olduğu için güvenliği en üstte tutuyoruz.
    // Tüm sınıf bazında sadece "Admin" rolüne sahip kullanıcılar işlem yapabilir.
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IAuditService _auditService;

        public UsersController(IAuthService authService, IAuditService auditService)
        {
            _authService = authService;
            _auditService = auditService;
        }

        // Sistemdeki mevcut personelleri (resepsiyon, admin vb.) listeler.
        public IActionResult Index() => View(_authService.GetAllUsers());

        // Yeni personel kayıt formunu açar.
        [HttpGet]
        public IActionResult Create() => View();

        // Yeni personeli kaydeder.
        [HttpPost]
        public IActionResult Create(User user)
        {
            if (!ModelState.IsValid) return View(user); // Form doğrulaması başarısızsa aynı sayfaya dön.
            
            // Veritabanına ekle
            _authService.AddUser(user);
            
            // Güvenlik izini logla
            _auditService.Log("Kullanıcı Yönetimi", $"Yeni personel oluşturuldu: {user.Username} (Rol: {user.Role})", User.Identity?.Name ?? "Admin", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1");
            
            return RedirectToAction(nameof(Index));
        }

        // Personel hesabını kalıcı olarak siler.
        [HttpPost]
        public IActionResult Delete(int id)
        {
            _authService.DeleteUser(id); // AuthService içindeki kural gereği ID:1 (Ana Admin) silinemez.
            
            _auditService.Log("Kullanıcı Yönetimi", $"ID: {id} olan personel sistemden silindi.", User.Identity?.Name ?? "Admin", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1");
            
            return RedirectToAction(nameof(Index));
        }
    }
}