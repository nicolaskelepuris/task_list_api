using System.Reflection;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {
        }

        public DbSet<Vessel> Vessels { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Task> Tasks { get; set; }
    }
}