using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecurePass.Business.Services;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SecurePass.Web.Controllers
{
    // Sisteme giriş yapmamış kişilerin bu sayfaya erişmesini engelliyoruz.
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IVisitorService _visitorService;
        private readonly IAuditService _auditService;
        
        // AI Fotoğraf indirmek için statik (tek bir instance) HttpClient kullanıyoruz. (Performans için)
        private static readonly HttpClient _httpClient;

        static DashboardController()
        {
            _httpClient = new HttpClient();
            // Bazı API'ler User-Agent olmadan engellediği için kendimizi standart bir tarayıcı gibi gösteriyoruz.
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
        }

        public DashboardController(IVisitorService visitorService, IAuditService auditService)
        {
            _visitorService = visitorService;
            _auditService = auditService;
        }

        // Dashboard ana sayfasını yükleyen metot.
        public IActionResult Index()
        {
            var all = _visitorService.GetAll();
            var today = DateTime.Today;

            // Analitik verilerini hesaplayıp arayüze (ViewBag ile) taşıyoruz.
            ViewBag.TotalVisitors = all.Count; // Toplam arşiv
            ViewBag.CurrentVisitors = all.Count(v => v.IsInside); // Şu an binada olanlar
            ViewBag.TodayVisitors = all.Count(v => v.VisitDate.Date == today); // Bugün girenler
            
            // Kapasite oranını %100'ü geçmeyecek şekilde hesaplıyoruz (Max 50 kişi varsaydık).
            ViewBag.OccupancyRate = Math.Min(100, (ViewBag.CurrentVisitors * 100) / 50);

            // Süre Aşımı (Overstay) kontrolü: Ziyaret saati + tahmini süre < şu anki zamansa süre aşılmıştır.
            var now = DateTime.Now;
            var overstayed = all.Where(v => v.IsInside && v.VisitDate.AddHours(v.ExpectedDurationHours) < now).ToList();
            ViewBag.OverstayCount = overstayed.Count;

            // Saatlik yoğunluk ısı haritası için veri hesaplama (08:00 ile 18:00 arası)
            var hourlyData = new int[11];
            foreach (var v in all.Where(x => x.VisitDate.Date == today))
            {
                int hour = v.VisitDate.Hour;
                if (hour >= 8 && hour <= 18) hourlyData[hour - 8]++;
            }
            ViewBag.HourlyStats = hourlyData;

            // Kurumsal / Bireysel ziyaretçi dağılımı (Pasta grafik için)
            ViewBag.CorporateCount = all.Count(v => !string.IsNullOrEmpty(v.Company));
            ViewBag.IndividualCount = all.Count(v => string.IsNullOrEmpty(v.Company));

            // Son 8 hareketi tabloya basmak için getiriyoruz.
            var recent = all.OrderByDescending(v => v.VisitDate).Take(8).ToList();
            return View(recent);
        }

        // Test Verisi Oluşturma (Sahte Veri) - Sadece Admin çalıştırabilir.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SeedData()
        {
            // Rastgele veri dizileri
            var maleNames = new[] { "Ahmet", "Mehmet", "Can", "Burak", "Mustafa", "Ali", "Hakan", "Emre", "Okan", "Serkan" };
            var femaleNames = new[] { "Ayşe", "Fatma", "Ece", "Selin", "Zeynep", "Merve", "Derya", "Gözde", "İrem", "Buse" };
            var lastNames = new[] { "Yılmaz", "Kaya", "Demir", "Çelik", "Şahin", "Öztürk", "Arslan", "Doğan", "Kılıç", "Aydın" };
            var companies = new[] { "Aselsan", "Turkcell", "Havelsan", "SoftTech", "Global Güvenlik", "ABC Holding", "XYZ Ltd.", "TeknoPark" };
            var departments = new[] { "Yönetim", "Bilgi İşlem", "İnsan Kaynakları", "Satın Alma", "Muhasebe", "Hukuk" };
            var purposes = new[] { "Haftalık Olağan Toplantı", "Teknik Arıza Bakım", "İş Mülakatı", "Sözleşme İmza Töreni", "Denetim ve Kontrol", "Ziyaret" };
            var hosts = new[] { "Murat Yıldız", "Selin Kaya", "Fatih Demir", "Zeynep Aydın", "Ahmet Şahin", "Ece Aksoy" };
            
            var random = new Random();
            for (int i = 0; i < 10; i++)
            {
                // Rastgele cinsiyet seçimi
                bool isMale = random.Next(0, 2) == 0;
                string firstName = isMale ? maleNames[random.Next(maleNames.Length)] : femaleNames[random.Next(femaleNames.Length)];
                
                byte[]? photoBytes = null;
                try 
                {
                    // Seçilen cinsiyete göre AI yüz üretici API'sinden fotoğraf (BLOB) indiriyoruz.
                    string gender = isMale ? "male" : "female";
                    photoBytes = await _httpClient.GetByteArrayAsync($"https://xsgames.co/randomusers/avatar.php?g={gender}");
                }
                catch { } // İnternet sorunu olursa kod çökmesin, fotosuz kaydetsin.

                // Yeni ziyaretçi Entity'sini dolduruyoruz
                var visitor = new SecurePass.Core.Entities.Visitor
                {
                    FirstName = firstName,
                    LastName = lastNames[random.Next(lastNames.Length)],
                    IdentityNumber = random.Next(100, 999).ToString() + random.Next(100, 999).ToString() + random.Next(10000, 99999).ToString(),
                    Company = companies[random.Next(companies.Length)],
                    PhoneNumber = "05" + random.Next(30, 56).ToString() + random.Next(1000000, 9999999).ToString(),
                    Email = firstName.ToLower() + "@" + (companies[random.Next(companies.Length)]?.ToLower() ?? "mail") + ".com",
                    VisitPurpose = purposes[random.Next(purposes.Length)],
                    HostPerson = hosts[random.Next(hosts.Length)],
                    Department = departments[random.Next(departments.Length)],
                    VehiclePlate = random.Next(1, 81).ToString("D2") + " " + (char)random.Next(65, 91) + (char)random.Next(65, 91) + " " + random.Next(100, 9999).ToString(),
                    
                    // Alarm sistemini simüle etmek için ilk 2 kaydın giriş saatini 6 saat öncesi yapıyoruz. (Overstay)
                    VisitDate = i < 2 ? DateTime.Now.AddHours(-6) : DateTime.Now.AddMinutes(-random.Next(10, 300)),
                    ExpectedDurationHours = random.Next(1, 9),
                    IsInside = true,
                    IsVip = random.Next(0, 10) == 1, // %10 ihtimalle VIP
                    IsBlacklisted = random.Next(0, 30) == 1, // Düşük ihtimalle Blacklist
                    AgreementAccepted = true,
                    Photo = photoBytes // İndirilen BLOB resmi
                };
                
                // Servis üzerinden veritabanına ekle
                _visitorService.Add(visitor);
            }
            
            // İşlemi Logla
            _auditService.Log("Sistem İşlemi", "Admin tarafından 10 adet cinsiyet uyumlu test verisi oluşturuldu.", User.Identity?.Name ?? "Admin", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1");
            
            return RedirectToAction("Index");
        }

        // Tüm veritabanını sıfırlayan Master Reset metodu. Sadece Admin erişebilir.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult ClearAllData()
        {
            _visitorService.ClearAll();
            _auditService.Log("Veri Temizleme", "Admin tarafından tüm ziyaretçi arşivi kalıcı olarak sıfırlandı.", User.Identity?.Name ?? "Admin", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1");
            return RedirectToAction("Index");
        }
    }
}