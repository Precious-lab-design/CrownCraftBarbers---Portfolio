using CrownCraftBarbers.Models;
using Microsoft.EntityFrameworkCore;

namespace CrownCraftBarbers.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Service> Services { get; set; }

        public DbSet<Barber> Barbers { get; set; }

        public DbSet<Booking> Bookings { get; set; }
    }
}