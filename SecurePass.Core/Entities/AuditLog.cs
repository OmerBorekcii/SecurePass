using System;
using System.ComponentModel.DataAnnotations;

namespace SecurePass.Core.Entities
{
    // Sistemdeki tüm önemli hareketlerin "İz Kayıtlarını" burada tutuyoruz.
    // Kim, ne zaman, hangi IP'den ne yaptı? Sorularının cevabı bu tabloda.
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        public string Action { get; set; } = string.Empty; // Eylem adı (örn: Giriş Yapıldı)

        public string Details { get; set; } = string.Empty; // Yapılan işlemin detayı

        public DateTime Timestamp { get; set; } = DateTime.Now; // İşlem zamanı

        public string Username { get; set; } = string.Empty; // İşlemi yapan kullanıcı

        public string IpAddress { get; set; } = string.Empty; // Cihazın IP adresi
    }
}