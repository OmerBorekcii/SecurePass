using System.ComponentModel.DataAnnotations;

namespace SecurePass.Core.Entities
{
    // Sisteme giriş yapacak olan personellerin (Admin/Resepsiyon) tablosu.
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Reception"; // Yetkilendirme için "Admin" veya "Reception"

        public bool IsActive { get; set; } = true; // Hesabı dondurmak gerekirse diye.
    }
}