using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.System;
using Windows.UI;


// Install Pomelo.EntityFrameworkCore.MySql


namespace bijzonderHandig.Data
{
    internal class AppDbContext : DbContext
    {

        // Niet vergeten om DbSet's toe te voegen voor elke entiteit die je wilt gebruiken in de database
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(
                    "server=localhost;" +
                    "database=" +
                    "MakersMarkt;" +
                    "user=root;" +
                    "password=;",
                    ServerVersion.Parse("8.0.30")
                );
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Users
            modelBuilder.Entity<User>().HasData(
            //new User { Id = 1, Username = "admin", Password = BCrypt.Net.BCrypt.HashPassword("admin123"), Role = "Admin", Status = "Verified" },
            );
        }
    }
}
