using Microsoft.EntityFrameworkCore;
using MyServer.Models;

namespace MyServer.BDConnection
{
    internal class BDContext : DbContext
    {
        public DbSet <Product> Products { get; set; }
        public DbSet <Category> Categorys { get; set; }
        public DbSet <MySupply> MySupplies { get; set; }
        public DbSet <Dish> Dishes { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-MV43C0T;Database=Product manager;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }
}
