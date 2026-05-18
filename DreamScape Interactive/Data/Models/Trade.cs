using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DreamScape_Interactive.Data.Models
{
    public class Trade
    {
        public int Id { get; set; }
   
        public int Player1Id { get; set; }
        
        public int Player2Id { get; set; }
   
        public int Player1ItemId { get; set; }
   
        public int Player1Quantity { get; set; }
   
        public int Player2ItemId { get; set; }
    
        public int Player2Quantity { get; set; }
    
        public int XPAwarded { get; set; } = 50; // XP both players receive
        
        public DateTime TradeDate { get; set; }
    }
}
