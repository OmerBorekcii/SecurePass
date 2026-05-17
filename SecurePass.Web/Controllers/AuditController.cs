using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecurePass.Business.Services;

namespace SecurePass.Web.Controllers
{
    // Audit (Denetim İzi) paneline sadece Adminler girebilir. 
    // Sistemin dijital kara kutusu buradadır.
    [Authorize(Roles = "Admin")]
    public class AuditController : Controller
    {
        private readonly IAuditService _auditService;
        
        public AuditController(IAuditService auditService) => _auditService = auditService;

        // Log kayıtlarını çeker ve sayfaya gönderir.
        public IActionResult Index()
        {
            var logs = _auditService.GetAll();
            return View(logs);
        }
    }
}