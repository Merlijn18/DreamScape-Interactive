using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DreamScape_Interactive.Data.Models
{
    public class TradeRequest
    {
        public int Id { get; set; }
        
        public int SenderId { get; set; }
        
        public int ReceiverId { get; set; }
        
      public int SenderItemId { get; set; }
   
    public int SenderQuantity { get; set; }
        
        public int ReceiverItemId { get; set; }
      
     public int ReceiverQuantity { get; set; }
 
      public string Status { get; set; } = "Pending"; // Pending, Accepted, Declined, Cancelled
        
        public DateTime CreatedDate { get; set; }
        
        public DateTime? ResponseDate { get; set; }
    }
}
