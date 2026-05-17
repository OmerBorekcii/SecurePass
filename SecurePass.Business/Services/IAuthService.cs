using System.Collections.Generic;
using SecurePass.Core.Entities;

namespace SecurePass.Business.Services
{
    // Auth Sözleşmesi: Kimlik doğrulama ve kullanıcı yönetim kurallarını belirler.
    public interface IAuthService
    {
        User? Authenticate(string username, string password);
        List<User> GetAllUsers();
        void AddUser(User user);
        void DeleteUser(int id);
    }
}