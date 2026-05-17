using System.ComponentModel.DataAnnotations;
using System;

namespace SecurePass.Core.Entities
{
    // Bu bizim ana Ziyaretçi tablomuz. 
    // Veritabanında ziyaretçilere ait hangi bilgileri tutacağımızı burada belirliyoruz.
    public class Visitor
    {
        [Key] // Bu alanın Primary Key (Birincil Anahtar) olduğunu belirtiyoruz.
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad alanı zorunludur.")] // Boş bırakılamaz uyarısı
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "TC Kimlik No zorunludur.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 haneli olmalıdır.")]
        public string IdentityNumber { get; set; } = string.Empty;

        public string? Company { get; set; } // Soru işareti (?) bu alanın NULL olabileceğini gösterir.
        
        [Required(ErrorMessage = "Telefon numarası zorunludur.")]
        public string PhoneNumber { get; set; } = string.Empty;

        public string? Email { get; set; }
        
        [Required(ErrorMessage = "Ziyaret nedeni zorunludur.")]
        public string VisitPurpose { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Ziyaret edilecek kişi/birim zorunludur.")]
        public string HostPerson { get; set; } = string.Empty;

        public string? Department { get; set; }
        
        public string? VehiclePlate { get; set; } // Otopark güvenliği için plaka bilgisi
        public string? Note { get; set; }
        
        public DateTime VisitDate { get; set; } = DateTime.Now; // Giriş saati, varsayılan olarak o anki zaman.
        public DateTime? ExitDate { get; set; } // Çıkış saati, kişi henüz çıkmadıysa null olur.

        public bool IsInside { get; set; } = true; // Kişi şu an binada mı?
        public bool IsVip { get; set; } = false; // Özel misafir etiketi
        public bool IsBlacklisted { get; set; } = false; // İstenmeyen kişi uyarısı
        public bool AgreementAccepted { get; set; } = false; // KVKK onayı alındı mı?
        
        public int ExpectedDurationHours { get; set; } = 4; // Tahmini kalış süresi (Alarm için)
        
        // Bu alanlar denetim (audit) için. İşlemi yapan personelin ID'sini tutuyoruz.
        public int? CreatedByUserId { get; set; }
        public int? UpdatedByUserId { get; set; }
        
        [Timestamp] // İyimser eşzamanlılık (Optimistic Concurrency) için EF Core'un kullandığı sürüm alanı.
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
        
        // Önemli: Fotoğraf verisini dosya yolu olarak değil, doğrudan byte dizisi (BLOB) olarak tutuyoruz.
        public byte[]? Photo { get; set; }
    }
}