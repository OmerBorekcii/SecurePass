using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SecurePass.Web.Hubs
{
    // SignalR Hub: Sunucu ile istemciler (tarayıcılar) arasında WebSockets üzerinden 
    // çift yönlü canlı (Real-time) iletişim kurmamızı sağlayan sınıftır.
    public class NotificationHub : Hub
    {
        // Yeni bir ziyaretçi kaydedildiğinde Controller bu metodu veya benzerini tetikler.
        // Hub da bağlı olan tüm aktif tarayıcılara "ReceiveNotification" sinyali gönderir.
        public async Task SendNotification(string message)
        {
            // Clients.All demek: Şu an sisteme bağlı olan herkese (Admin, Resepsiyon) bu mesajı yayınla demektir.
            await Clients.All.SendAsync("ReceiveNotification", message);
        }
    }
}