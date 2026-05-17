using System.Collections.Generic;
using SecurePass.Core.Entities;

namespace SecurePass.Business.Services
{
    // Interface (Arayüz): Ziyaretçi servisimizin dış dünyaya açılan sözleşmesidir.
    // S.O.L.I.D. prensiplerinden Dependency Inversion'ı (Bağımlılıkları Tersine Çevirme) sağlamak için kullanılır.
    // Controller'lar sınıfın kendisine değil, bu arayüze bağımlı olur.
    public interface IVisitorService
    {
        // Tüm ziyaretçileri getiren metot sözleşmesi
        List<Visitor> GetAll();
        
        // Arama motoru için metot sözleşmesi
        List<Visitor> Search(string keyword);
        
        // Tek bir ziyaretçiyi ID'ye göre bulma sözleşmesi
        Visitor? GetById(int id);
        
        // Yeni kayıt ekleme sözleşmesi
        void Add(Visitor visitor);
        
        // Mevcut kaydı güncelleme sözleşmesi
        void Update(Visitor visitor);
        
        // Ziyaretçinin çıkış işlemini (IsInside = false) yapacak metot sözleşmesi
        void CheckOut(int id);
        
        // Toplu çıkış işlemi sözleşmesi (Performans optimizasyonu için)
        void BulkCheckOut(int[] ids);
        
        // Tüm veritabanını temizleme sözleşmesi (Master Reset)
        void ClearAll();
        
        // Belirli bir kaydı tamamen silme sözleşmesi (Hard Delete)
        void Delete(int id);
    }
}