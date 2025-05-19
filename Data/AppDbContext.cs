using Microsoft.EntityFrameworkCore;
using FoodApi.Models;

namespace FoodApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Destination> Destinations { get; set; }
    }
}
