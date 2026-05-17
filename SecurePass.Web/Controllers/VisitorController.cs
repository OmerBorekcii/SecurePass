using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SecurePass.Business.Services;
using SecurePass.Core.Entities;
using SecurePass.Web.Hubs;
using System.IO;
using System.Threading.Tasks;

namespace SecurePass.Web.Controllers
{
    [Authorize] // Bu sınıfa sadece giriş yapmış kişiler erişebilir.
    public class VisitorController : Controller
    {
        private readonly IVisitorService _visitorService;
        private readonly IAuditService _auditService;
        private readonly IHubContext<NotificationHub> _hubContext; // SignalR ile anlık bildirim göndermek için

        // Dependency Injection sayesinde servislerimizi içeri alıyoruz.
        public VisitorController(IVisitorService visitorService, IAuditService auditService, IHubContext<NotificationHub> hubContext)
        {
            _visitorService = visitorService;
            _auditService = auditService;
            _hubContext = hubContext;
        }

        // Ziyaretçi Listesi ve Filtreleme sayfası
        public IActionResult Index(string keyword)
        {
            var visitors = _visitorService.Search(keyword);
            ViewBag.Keyword = keyword; // Arama kutusunda yazdığımız kelime kalsın diye.
            return View(visitors);
        }

        [HttpGet]
        public IActionResult Create() => View(new Visitor());

        [HttpPost]
        public async Task<IActionResult> Create(Visitor visitor, IFormFile? photoFile)
        {
            // İSTER: Fotoğraf yükleme şartı kontrolü
            if (photoFile == null || photoFile.Length == 0)
            {
                ModelState.AddModelError("", "Güvenlik politikası gereği ziyaretçi fotoğrafı zorunludur.");
            }

            if (!ModelState.IsValid) return View(visitor);

            // İşlemi yapan personelin ID'sini alıyoruz (Cookie'den)
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                visitor.CreatedByUserId = userId;
            }

            // İSTER: Resmi dosya yolu değil, BLOB (byte[]) olarak saklama işlemi
            if (photoFile != null && photoFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await photoFile.CopyToAsync(memoryStream);
                visitor.Photo = memoryStream.ToArray(); // Resim artık veritabanına hazır!
            }

            _visitorService.Add(visitor);

            // Audit Trail: Bu kaydı kimin yaptığını satır satır logluyoruz.
            _auditService.Log("Ziyaretçi Kaydı", $"{visitor.FirstName} {visitor.LastName} (TC: {visitor.IdentityNumber}) binaya giriş yaptı.", User.Identity?.Name ?? "Sistem", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0");
            
            // SignalR: Admin ekranına sayfa yenilenmeden bildirim gönderiyoruz.
            try {
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", $"🔔 Yeni Ziyaretçi: {visitor.FirstName} {visitor.LastName}");
            } catch { }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult CheckOut(int id)
        {
            _visitorService.CheckOut(id);
            _auditService.Log("Çıkış İşlemi", $"ID: {id} olan ziyaretçi çıkış yaptı.", User.Identity?.Name ?? "Sistem", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult BulkCheckOut(int[] visitorIds)
        {
            if (visitorIds != null && visitorIds.Length > 0)
            {
                _visitorService.BulkCheckOut(visitorIds);
                _auditService.Log("Toplu Çıkış", $"{visitorIds.Length} kişi sistemden toplu olarak çıkarıldı.", User.Identity?.Name ?? "Sistem", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0");
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // Sadece Admin rolüne sahip olanlar bu metodu çalıştırabilir.
        public IActionResult Delete(int id)
        {
            _visitorService.Delete(id);
            _auditService.Log("Veri İmhası", $"ID: {id} olan kayıt kalıcı olarak silindi (Hard Delete).", User.Identity?.Name ?? "Sistem", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0");
            return RedirectToAction(nameof(Index));
        }
    }
}