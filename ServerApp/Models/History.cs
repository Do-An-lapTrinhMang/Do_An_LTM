using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.Models
{
    public class History
    {
        public int HistoryID { get; set; }    

        public int SenderID { get; set; }     
        public int ReceiverID { get; set; }  

        public string FileName { get; set; }  
        public long FileSize { get; set; }    

        public DateTime Timestamp { get; set; } 

        public string Status { get; set; }    

        // Navigation properties
        public virtual User Sender { get; set; }
        public virtual User Receiver { get; set; }
    }
}
