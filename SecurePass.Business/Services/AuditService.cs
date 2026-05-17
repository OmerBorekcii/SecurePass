using SecurePass.Core.Entities;
using SecurePass.DataAccess.Context;
using System.Collections.Generic;
using System.Linq;

namespace SecurePass.Business.Services
{
    // Sistemin "Kara Kutusu". Yapılan her hareketi veritabanına kaydeder.
    public class AuditService : IAuditService
    {
        private readonly SecurePassContext _context;

        public AuditService(SecurePassContext context)
        {
            _context = context;
        }

        // Yeni bir log (iz) kaydı oluşturur.
        public void Log(string action, string details, string username, string ip)
        {
            var log = new AuditLog 
            { 
                Action = action, // Eylem tipi (Örn: Veri Silme)
                Details = details, // Açıklama
                Username = username, // Yapan kişi
                IpAddress = ip, // İşlemin yapıldığı IP
                Timestamp = System.DateTime.Now // İşlem anı
            };
            _context.AuditLogs.Add(log);
            _context.SaveChanges(); // Logu veritabanına yaz.
        }

        // Tüm logları tarihe göre (en yeni en üstte) sıralayarak getirir.
        public List<AuditLog> GetAll()
        {
            return _context.AuditLogs.OrderByDescending(l => l.Timestamp).ToList();
        }
    }
}