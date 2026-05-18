using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DreamScape_Interactive.Data.Models;

namespace DreamScape_Interactive.Data
{
    internal class AppDbContext : DbContext
    {
      public DbSet<User> Users { get; set; }
     public DbSet<Item> Items { get; set; }
        public DbSet<PlayerItem> PlayerItems { get; set; }
        public DbSet<TradeRequest> TradeRequests { get; set; }
        public DbSet<TradeRequestItem> TradeRequestItems { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<TradeItem> TradeItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                    "server=localhost;" +
                      "user=root;" +
            "password=;" +
                            "database=DreamScape_Interactive",
                 ServerVersion.Parse("8.0.30")
                        );
        }
    
     //Seeders
        protected override void OnModelCreating(ModelBuilder modelBuilder)
 {
            base.OnModelCreating(modelBuilder);

       // === USERS SEED DATA ===
            modelBuilder.Entity<User>().HasData(
     // Admin Users
     new User 
        { 
               Id = 1, 
           Username = "Admin", 
   Email = "admin@dreamscape.com", 
          Password = BCrypt.Net.BCrypt.HashPassword("admin123"), 
          Role = "Beheer", 
        CreatedAt = DateTime.Now.AddDays(-30),
     Level = 10,
       CurrentXP = 500,
        XPToNextLevel = 1000
      },
    new User 
      { 
         Id = 2, 
       Username = "Merlijn", 
     Email = "merlijn@dreamscape.com", 
           Password = BCrypt.Net.BCrypt.HashPassword("123456"), 
    Role = "Beheer", 
  CreatedAt = DateTime.Now.AddDays(-25),
          Level = 8,
           CurrentXP = 350,
           XPToNextLevel = 800
            },
  
    // Player Users
  new User 
  { 
            Id = 3, 
         Username = "DragonSlayer", 
             Email = "dragon@player.com", 
          Password = BCrypt.Net.BCrypt.HashPassword("123456"), 
            Role = "Player", 
        CreatedAt = DateTime.Now.AddDays(-20),
                 Level = 15,
  CurrentXP = 750,
         XPToNextLevel = 1500
         },
                new User 
          { 
             Id = 4, 
         Username = "ShadowNinja", 
    Email = "shadow@player.com", 
            Password = BCrypt.Net.BCrypt.HashPassword("123456"), 
         Role = "Player", 
       CreatedAt = DateTime.Now.AddDays(-18),
   Level = 12,
                CurrentXP = 600,
           XPToNextLevel = 1200
     },
       new User 
    { 
          Id = 5, 
      Username = "MysticMage", 
    Email = "mystic@player.com", 
         Password = BCrypt.Net.BCrypt.HashPassword("123456"), 
  Role = "Player", 
          CreatedAt = DateTime.Now.AddDays(-15),
        Level = 10,
         CurrentXP = 450,
    XPToNextLevel = 1000
      },
          new User 
   { 
    Id = 6, 
    Username = "IronKnight", 
   Email = "iron@player.com", 
      Password = BCrypt.Net.BCrypt.HashPassword("123456"), 
      Role = "Player", 
          CreatedAt = DateTime.Now.AddDays(-12),
        Level = 8,
          CurrentXP = 300,
               XPToNextLevel = 800
           },
    new User 
  { 
        Id = 7, 
                  Username = "FirePhoenix", 
      Email = "phoenix@player.com", 
          Password = BCrypt.Net.BCrypt.HashPassword("123456"), 
   Role = "Player", 
     CreatedAt = DateTime.Now.AddDays(-10),
      Level = 7,
          CurrentXP = 280,
                XPToNextLevel = 700
 },
                new User 
                { 
           Id = 8, 
    Username = "StormBreaker", 
Email = "storm@player.com", 
         Password = BCrypt.Net.BCrypt.HashPassword("123456"), 
            Role = "Player", 
         CreatedAt = DateTime.Now.AddDays(-8),
    Level = 6,
   CurrentXP = 200,
 XPToNextLevel = 600
         },
    new User 
    { 
        Id = 9, 
   Username = "NightHunter", 
         Email = "hunter@player.com", 
   Password = BCrypt.Net.BCrypt.HashPassword("123456"), 
       Role = "Player", 
      CreatedAt = DateTime.Now.AddDays(-5),
   Level = 5,
  CurrentXP = 150,
         XPToNextLevel = 500
      },
      new User 
      { 
          Id = 10, 
        Username = "CrystalArcher", 
    Email = "crystal@player.com", 
       Password = BCrypt.Net.BCrypt.HashPassword("123456"), 
 Role = "Player", 
      CreatedAt = DateTime.Now.AddDays(-3),
           Level = 4,
        CurrentXP = 120,
           XPToNextLevel = 400
    },
       new User 
            { 
   Id = 11, 
          Username = "TestPlayer", 
             Email = "test@player.com", 
         Password = BCrypt.Net.BCrypt.HashPassword("123456"), 
           Role = "Player", 
               CreatedAt = DateTime.Now.AddDays(-1),
      Level = 3,
       CurrentXP = 80,
         XPToNextLevel = 300
     }
  );

  // === ITEMS SEED DATA ===
   modelBuilder.Entity<Item>().HasData(
   // Weapons
       new Item
         {
         Id = 1,
         Name = "Flame Sword",
    Description = "A legendary sword infused with eternal fire magic.",
     Type = "Weapon",
         Rarity = "Epic",
 strength = 25,
        Speed = 10,
                  Durability = 40,
         Magic_Effect = "Burn"
  },
   new Item
     {
          Id = 2,
             Name = "Shadow Dagger",
    Description = "A deadly dagger used by master assassins.",
 Type = "Weapon",
   Rarity = "Rare",
      strength = 15,
       Speed = 30,
       Durability = 20,
            Magic_Effect = "Stealth Boost"
  },
         new Item
  {
          Id = 3,
        Name = "Mystic Staff",
            Description = "An ancient staff channeling pure magical energy.",
       Type = "Weapon",
       Rarity = "Epic",
 strength = 18,
             Speed = 8,
         Durability = 30,
     Magic_Effect = "Mana Boost"
      },
                new Item
                {
         Id = 4,
      Name = "Thunder Hammer",
        Description = "A massive hammer crackling with lightning.",
          Type = "Weapon",
      Rarity = "Legendary",
           strength = 35,
            Speed = 5,
       Durability = 50,
      Magic_Effect = "Lightning Strike"
         },
        new Item
       {
     Id = 5,
       Name = "Crystal Bow",
              Description = "An elegant bow made from enchanted crystals.",
        Type = "Weapon",
       Rarity = "Rare",
  strength = 20,
     Speed = 25,
            Durability = 30,
      Magic_Effect = "Piercing Shot"
     },
        
      // Armor
    new Item
          {
   Id = 6,
            Name = "Dragon Armor",
             Description = "Legendary armor forged from ancient dragon scales.",
            Type = "Armor",
         Rarity = "Legendary",
            strength = 10,
        Speed = -5,
        Durability = 80,
   Magic_Effect = "Fire Resistance"
            },
      new Item
                {
            Id = 7,
           Name = "Wind Boots",
   Description = "Magical boots that harness the power of wind.",
 Type = "Armor",
  Rarity = "Rare",
            strength = 0,
     Speed = 20,
       Durability = 25,
      Magic_Effect = "Wind Dash"
       },
     new Item
          {
        Id = 8,
      Name = "Shadow Cloak",
      Description = "A dark cloak that grants stealth abilities.",
        Type = "Armor",
  Rarity = "Epic",
           strength = 5,
 Speed = 15,
           Durability = 35,
      Magic_Effect = "Invisibility"
     },
  new Item
       {
          Id = 9,
   Name = "Iron Shield",
            Description = "A sturdy shield providing excellent protection.",
      Type = "Armor",
 Rarity = "Common",
   strength = 15,
 Speed = -10,
  Durability = 60,
          Magic_Effect = "Block"
   },
     new Item
    {
           Id = 10,
       Name = "Mana Crown",
        Description = "A crown that enhances magical abilities.",
    Type = "Armor",
             Rarity = "Epic",
                    strength = 0,
Speed = 0,
      Durability = 40,
            Magic_Effect = "Mana Regeneration"
  }
            );

            // === PLAYER ITEMS SEED DATA ===
    modelBuilder.Entity<PlayerItem>().HasData(
        // Admin inventories
    new PlayerItem { Id = 1, UserId = 1, ItemId = 1, Quantity = 2 },
  new PlayerItem { Id = 2, UserId = 1, ItemId = 6, Quantity = 1 },
       new PlayerItem { Id = 3, UserId = 2, ItemId = 3, Quantity = 1 },
      new PlayerItem { Id = 4, UserId = 2, ItemId = 8, Quantity = 1 },
     
       // Player inventories
       new PlayerItem { Id = 5, UserId = 3, ItemId = 4, Quantity = 1 },
        new PlayerItem { Id = 6, UserId = 3, ItemId = 6, Quantity = 1 },
      new PlayerItem { Id = 7, UserId = 3, ItemId = 9, Quantity = 2 },
            
                new PlayerItem { Id = 8, UserId = 4, ItemId = 2, Quantity = 3 },
 new PlayerItem { Id = 9, UserId = 4, ItemId = 8, Quantity = 1 },
         
      new PlayerItem { Id = 10, UserId = 5, ItemId = 3, Quantity = 2 },
            new PlayerItem { Id = 11, UserId = 5, ItemId = 10, Quantity = 1 },
        new PlayerItem { Id = 12, UserId = 5, ItemId = 7, Quantity = 1 },
  
      new PlayerItem { Id = 13, UserId = 6, ItemId = 9, Quantity = 1 },
                new PlayerItem { Id = 14, UserId = 6, ItemId = 6, Quantity = 1 },
      
         new PlayerItem { Id = 15, UserId = 7, ItemId = 1, Quantity = 1 },
          new PlayerItem { Id = 16, UserId = 7, ItemId = 8, Quantity = 1 },
    
                new PlayerItem { Id = 17, UserId = 8, ItemId = 4, Quantity = 1 },
     new PlayerItem { Id = 18, UserId = 8, ItemId = 7, Quantity = 2 },
          
      new PlayerItem { Id = 19, UserId = 9, ItemId = 5, Quantity = 1 },
           new PlayerItem { Id = 20, UserId = 9, ItemId = 7, Quantity = 1 },
      
     new PlayerItem { Id = 21, UserId = 10, ItemId = 5, Quantity = 2 },
  new PlayerItem { Id = 22, UserId = 10, ItemId = 10, Quantity = 1 },
            
 new PlayerItem { Id = 23, UserId = 11, ItemId = 2, Quantity = 1 },
              new PlayerItem { Id = 24, UserId = 11, ItemId = 7, Quantity = 1 },
        new PlayerItem { Id = 25, UserId = 11, ItemId = 9, Quantity = 1 }
            );
        }
}
}
