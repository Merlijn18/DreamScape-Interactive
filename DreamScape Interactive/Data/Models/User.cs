using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DreamScape_Interactive.Data.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = "Player";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int Level { get; set; } = 1;

        public int CurrentXP { get; set; } = 0;

        public int XPToNextLevel { get; set; } = 100;

        public DateTime? LastDailyClaim { get; set; }

        public static User? LoggedInUser { get; set; }
    }
}
