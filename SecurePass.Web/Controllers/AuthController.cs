using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SecurePass.Business.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.RateLimiting;

namespace SecurePass.Web.Controllers
{
    // Kimlik Doğrulama (Authentication) işlemlerinin yürütüldüğü Controller.
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IAuditService _auditService;

        // DI Container üzerinden servisleri alıyoruz.
        public AuthController(IAuthService authService, IAuditService auditService)
        {
            _authService = authService;
            _auditService = auditService;
        }

        // Login sayfasını ekrana basan HTTP GET metodu.
        [HttpGet]
        public IActionResult Login() => View();

        // Form post edildiğinde (Giriş yap butonuna basıldığında) çalışan HTTP POST metodu.
        [HttpPost]
        [EnableRateLimiting("LoginLimiter")] // Brute-force saldırılarını engellemek için IP bazlı istek sınırı koyar.
        public async Task<IActionResult> Login(string username, string password)
        {
            // Kullanıcıyı veritabanında doğruluyoruz.
            var user = _authService.Authenticate(username, password);

            // Eğer kullanıcı yoksa veya şifre yanlışsa:
            if (user == null)
            {
                // Güvenlik İhlali olarak Audit log'a kaydediyoruz.
                _auditService.Log("Güvenlik İhlali", $"Hatalı giriş denemesi. Kullanıcı: {username}", "Misafir", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0");
                ViewBag.Error = "Geçersiz kullanıcı adı veya şifre.";
                return View(); // Tekrar login sayfasına gönder.
            }

            // Doğrulama başarılıysa Kullanıcı Kimlik Kartını (Claims) oluşturuyoruz.
            var claims = new List<Claim> 
            { 
                new Claim(ClaimTypes.Name, user.Username), // İsim
                new Claim(ClaimTypes.Role, user.Role),     // Yetki Rolü (Admin/Reception)
                new Claim("UserId", user.Id.ToString())    // Veritabanı ID'si
            };
            
            // Kimlik kartını Cookie (Çerez) şemasına uygun hale getiriyoruz.
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            
            // Kullanıcıyı sisteme şifreli Cookie ile giriş yaptırıyoruz. (Oturum Açma)
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            // Başarılı girişi Audit log'a yazıyoruz.
            _auditService.Log("Sistem Erişimi", $"Kullanıcı başarıyla giriş yaptı.", user.Username, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0");

            // Yönetim Paneline (Dashboard) yönlendir.
            return RedirectToAction("Index", "Dashboard");
        }

        // Çıkış (Logout) işlemi
        public async Task<IActionResult> Logout()
        {
            var user = User.Identity?.Name ?? "Bilinmiyor";
            // Çıkış işlemini logluyoruz.
            _auditService.Log("Sistem Çıkışı", "Kullanıcı güvenli çıkış yaptı.", user, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0");
            
            // Tarayıcıdaki oturum çerezini siliyoruz.
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            // Tekrar Login sayfasına yönlendir.
            return RedirectToAction("Login");
        }
    }
}