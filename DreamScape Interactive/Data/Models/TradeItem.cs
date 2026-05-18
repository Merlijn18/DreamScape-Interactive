using System;

namespace DreamScape_Interactive.Data.Models
{
    public class TradeItem
    {
        public int Id { get; set; }
        public int TradeId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public bool IsPlayer1Giving { get; set; } // True = Player 1 gave this, False = Player 2 gave this
    }
}