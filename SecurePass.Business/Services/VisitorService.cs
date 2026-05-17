using System.Collections.Generic;
using System.Linq;
using SecurePass.Core.Entities;
using SecurePass.DataAccess.Context;

namespace SecurePass.Business.Services
{
    // Ziyaretçilerle ilgili tüm "iş mantığı" (Business Logic) burada dönüyor.
    // Controller doğrudan veritabanına gitmez, bu servisi kullanır.
    public class VisitorService : IVisitorService
    {
        private readonly SecurePassContext _context;

        public VisitorService(SecurePassContext context)
        {
            _context = context;
        }

        // Tüm ziyaretçileri tarihe göre tersten listele (En yeni en üstte)
        public List<Visitor> GetAll() => _context.Visitors.OrderByDescending(v => v.VisitDate).ToList();

        // Gelişmiş arama motorumuz. Birçok alanı tek bir keyword ile tarayabiliyoruz.
        public List<Visitor> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return GetAll();
            
            keyword = keyword.ToLower();
            return _context.Visitors.Where(v => 
                v.FirstName.ToLower().Contains(keyword) || 
                v.LastName.ToLower().Contains(keyword) || 
                v.IdentityNumber.Contains(keyword) || 
                (v.VehiclePlate != null && v.VehiclePlate.ToLower().Contains(keyword)) ||
                (v.Company != null && v.Company.ToLower().Contains(keyword)))
                .OrderByDescending(v => v.VisitDate).ToList();
        }

        public Visitor? GetById(int id) => _context.Visitors.Find(id);

        public void Add(Visitor visitor)
        {
            _context.Visitors.Add(visitor);
            _context.SaveChanges();
        }

        public void Update(Visitor visitor)
        {
            _context.Visitors.Update(visitor);
            _context.SaveChanges();
        }

        // Tekli çıkış işlemi
        public void CheckOut(int id)
        {
            var visitor = _context.Visitors.Find(id);
            if (visitor != null && visitor.IsInside)
            {
                visitor.IsInside = false;
                visitor.ExitDate = DateTime.Now;
                _context.SaveChanges();
            }
        }

        // Toplu çıkış işlemi. Veritabanına tek bir SaveChanges ile gitmek performans sağlar.
        public void BulkCheckOut(int[] ids)
        {
            var visitors = _context.Visitors.Where(v => ids.Contains(v.Id) && v.IsInside).ToList();
            foreach (var v in visitors)
            {
                v.IsInside = false;
                v.ExitDate = DateTime.Now;
            }
            _context.SaveChanges();
        }

        public void ClearAll()
        {
            _context.Visitors.RemoveRange(_context.Visitors);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var visitor = _context.Visitors.Find(id);
            if (visitor != null)
            {
                _context.Visitors.Remove(visitor);
                _context.SaveChanges();
            }
        }
    }
}