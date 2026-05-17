using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SecurePass.Business.Services;
using SecurePass.DataAccess.Context;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using SecurePass.Web.Middleware;

// 1. UYGULAMA İNŞA EDİCİSİ (BUILDER)
// ASP.NET Core uygulamamızın temel taşıdır. Tüm servisleri (Dependency Injection) buraya ekleriz.
var builder = WebApplication.CreateBuilder(args);

// 2. VERİTABANI BAĞLANTISI (SQL Server)
// appsettings.json dosyasından "DefaultConnection" bağlantı cümlesini okur ve EF Core'a verir.
builder.Services.AddDbContext<SecurePassContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. BAĞIMLILIK ENJEKSİYONU (Dependency Injection - DI)
// S.O.L.I.D. prensiplerine uygun olarak; Interface'leri (Sözleşme) Somut Sınıflarla eşleştiriyoruz.
// Scoped: Her HTTP isteğinde (Request) bu servislerden yeni bir tane yaratılır.
builder.Services.AddScoped<IVisitorService, VisitorService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuditService, AuditService>();

// 4. KİMLİK DOĞRULAMA (Authentication)
// Sisteme giriş yapacak personeller için Çerez (Cookie) tabanlı güvenlik duvarı oluşturuyoruz.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login"; // Yetkisiz biri girerse buraya yönlendir.
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied"; // Yetkisi yetmeyen (örn: Resepsiyon silmeye kalkarsa)
    });

// Temel MVC yapısı ve SignalR (Canlı Bildirim) servisleri
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

// 5. SİSTEM SAĞLIĞI (Health Checks)
// IT ekiplerinin veritabanının ayakta olup olmadığını kontrol edebilmesi için.
builder.Services.AddHealthChecks()
    .AddDbContextCheck<SecurePassContext>();

// 6. BRUTE-FORCE KORUMASI (Rate Limiting)
// Saniyede çok fazla giriş denemesi yaparak şifre kırmaya çalışan hackerları engellemek için.
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("LoginLimiter", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1); // 1 Dakikalık zaman dilimi
        opt.PermitLimit = 5; // Maksimum 5 giriş denemesi
        opt.QueueLimit = 0; // Kuyruk yok, direkt engelle (429 Too Many Requests)
    });
});

// BUILD: Tüm servisler eklendikten sonra uygulamayı derliyoruz.
var app = builder.Build();

// OTOMATİK MİGRASYON: Sunucuya yüklendiğinde veritabanını otomatik oluşturur.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<SecurePassContext>();
        context.Database.Migrate(); // Eksik tabloları ve veritabanını otomatik açar.
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Veritabanı migrasyonu sırasında hata oluştu.");
    }
}

// 7. MIDDLEWARE (ARA KATMAN) ZİNCİRİ
// Gelen her HTTP isteği (Request) bu boru hattından (Pipeline) geçer. Sırası çok önemlidir!

// Kendi yazdığımız özel Hata Yakalama katmanı. (Sarı hata ekranlarını engeller)
app.UseMiddleware<GlobalExceptionMiddleware>();

// Brute-force korumasını devreye al
app.UseRateLimiter();

// /health endpoint'ini dışa aç
app.MapHealthChecks("/health");

if (!app.Environment.IsDevelopment())
{
    app.UseHsts(); // Canlı ortamda zorunlu HTTPS yönlendirmesi
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // wwwroot klasöründeki css/js/resim dosyalarının dışa açılması
app.UseRouting();

// GÜVENLİK DUVARLARI
app.UseAuthentication(); // Kimsin? (Giriş yaptın mı)
app.UseAuthorization();  // Yetkin var mı? (Admin misin, Resepsiyon mu)

// Gelen isteği ilgili Controller'a yönlendir. (Varsayılan sayfa: Dashboard)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

// SignalR Canlı Yayın istasyonunu bağla
app.MapHub<SecurePass.Web.Hubs.NotificationHub>("/notificationHub");

// Motoru çalıştır!
app.Run();