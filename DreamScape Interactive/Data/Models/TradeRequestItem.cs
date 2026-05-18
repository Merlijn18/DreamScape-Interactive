using System;

namespace DreamScape_Interactive.Data.Models
{
    public class TradeRequestItem
    {
        public int Id { get; set; }
        public int TradeRequestId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public bool IsSenderOffer { get; set; } // True = Sender gives, False = Receiver gives
    }
}
