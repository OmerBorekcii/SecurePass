using SecurePass.Core.Entities;
using System.Collections.Generic;

namespace SecurePass.Business.Services
{
    // Denetim İzi (Audit) sözleşmesi.
    public interface IAuditService
    {
        void Log(string action, string details, string username, string ip);
        List<AuditLog> GetAll();
    }
}