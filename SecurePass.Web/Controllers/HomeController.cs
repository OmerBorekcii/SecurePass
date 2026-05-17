using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SecurePass.Web.Controllers;

// Sistemin varsayılan hatalarının ve boş sayfalarının yönetildiği yer.
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    // GlobalExceptionMiddleware hata yakaladığında kullanıcıyı bu sayfaya gönderir.
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(string? errorId)
    {
        // Gelen Hata Takip Kodunu (GUID) arayüze taşır. 
        // Kullanıcı bu kodu IT destek ekibine iletir.
        ViewBag.ErrorId = errorId;
        return View();
    }
}