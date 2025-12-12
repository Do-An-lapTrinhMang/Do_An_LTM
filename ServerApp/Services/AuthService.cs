using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ServerApp.Data;

namespace ServerApp.Services
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
        public AuthResult Register(string username, string password, string fullName)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    // Kiểm tra username đã tồn tại
                    if (db.Users.Any(u => u.Username == username))
                    {
                        return new AuthResult
                        {
                            Success = false,
                            ErrorMessage = "Username already exists"
                        };
                    }

                    // Hash password
                    string passwordHash = HashPassword(password);

                    // Tạo user mới
                    var user = new ClientApp.Models.User
                    {
                        Username = username,
                        PasswordHash = passwordHash,
                        FullName = fullName
                    };

                    db.Users.Add(user);
                    db.SaveChanges();

                    return new AuthResult
                    {
                        Success = true,
                        UserId = user.UserID,
                        FullName = user.FullName
                    };
                }
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public AuthResult Login(string username, string password)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    string passwordHash = HashPassword(password);
                    var user = db.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == passwordHash);

                    if (user != null)
                    {
                        return new AuthResult
                        {
                            Success = true,
                            UserId = user.UserID,
                            FullName = user.FullName
                        };
                    }

                    return new AuthResult
                    {
                        Success = false,
                        ErrorMessage = "Invalid username or password"
                    };
                }
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
