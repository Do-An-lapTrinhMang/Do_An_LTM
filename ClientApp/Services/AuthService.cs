using System;

namespace ClientApp.Services
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class AuthService
    {
        private readonly NetworkService _networkService;

        public AuthService()
        {
            _networkService = new NetworkService();
        }

        public AuthResult Login(string username, string password)
        {
            string request = $"LOGIN|{username}|{password}";
            string response = _networkService.SendRequest(request);
            
            string[] parts = response.Split('|');
            
            if (parts[0] == "OK" && parts.Length >= 3)
            {
                return new AuthResult
                {
                    Success = true,
                    UserId = int.Parse(parts[1]),
                    FullName = parts[2]
                };
            }
            
            return new AuthResult
            {
                Success = false,
                ErrorMessage = parts.Length > 1 ? parts[1] : "Đăng nhập thất bại!"
            };
        }

        public AuthResult Register(string username, string password, string fullName)
        {
            string request = $"REGISTER|{username}|{password}|{fullName}";
            string response = _networkService.SendRequest(request);
            
            string[] parts = response.Split('|');
            
            if (parts[0] == "OK")
            {
                return new AuthResult
                {
                    Success = true
                };
            }
            
            return new AuthResult
            {
                Success = false,
                ErrorMessage = parts.Length > 1 ? parts[1] : "Đăng ký thất bại!"
            };
        }
    }
}
