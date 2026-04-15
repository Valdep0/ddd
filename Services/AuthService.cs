using MasterPolApp.Models;

namespace MasterPolApp.Services
{
    public class AuthService
    {
        private DatabaseService _db;

        public AuthService()
        {
            _db = new DatabaseService();
        }

        public User Login(string username, string password)
        {
            return _db.Login(username, password);
        }

        public string GetRoleName(string role)
        {
            return _db.GetRoleName(role);
        }
    }
}