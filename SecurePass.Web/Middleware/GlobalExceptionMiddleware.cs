using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace SecurePass.Web.Middleware
{
    // Global Exception Middleware (Özel Hata Yakalama Katmanı):
    // Uygulama çalışırken Controller'larda veya Servislerde beklenmedik bir hata (Exception) çıkarsa,
    // ekranın çökmesi (Sarı YSOD ekranı) yerine bu aracı (Middleware) hatayı yakalar.
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next; // Uygulamanın çalışmaya devam etmesi için gerekli kanal.
        private readonly ILogger<GlobalExceptionMiddleware> _logger; // Arka planda log tutmak için.

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Önce uygulamanın normal akışına (sonraki katmanlara) izin ver.
                await _next(context);
            }
            catch (Exception ex)
            {
                // Eğer akış sırasında bir yer patlarsa catch bloğuna düşer.
                
                // Benzersiz bir Hata Takip Kodu (GUID) oluştur.
                var errorId = Guid.NewGuid().ToString();
                
                // Hatayı tüm teknik detaylarıyla arka planda (console/dosya) logla.
                _logger.LogError(ex, "Kritik Hata ID: {ErrorId} - Mesaj: {Message}", errorId, ex.Message);
                
                // Kullanıcıyı, teknik detayların gizlendiği, sadece Hata Kodunun yazdığı dostane bir sayfaya yönlendir.
                context.Response.Redirect($"/Home/Error?errorId={errorId}");
            }
        }
    }
}