using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Weather_Acknowledgement.Models
{
    public class WeatherDbContext : DbContext
    {
        public WeatherDbContext(DbContextOptions<WeatherDbContext> options)
        : base(options)
        {
        }

        public DbSet<WeatherStation> WeatherStations { get; set; }
        public DbSet<WeatherReading> WeatherReadings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeatherReading>()
                .HasOne(r => r.WeatherStation)
                .WithMany(s => s.Readings)
                .HasForeignKey(r => r.WeatherStationId);
        }
    }
}
