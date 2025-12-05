using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.Models
{
    public class User
    {
        public int UserID { get; set; }          
        public string Username { get; set; }    
        public string PasswordHash { get; set; } 
        public string FullName { get; set; }    

        // Quan hệ 1 User → nhiều lịch sử gửi file
        public virtual ICollection<History> SentHistories { get; set; }

        // Quan hệ 1 User → nhiều lịch sử nhận file
        public virtual ICollection<History> ReceivedHistories { get; set; }
    }
}
