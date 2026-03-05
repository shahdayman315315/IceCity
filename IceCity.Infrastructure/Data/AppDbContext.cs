using IceCity.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCity.Infrastructure.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<House> Houses { get; set; }
        public DbSet<Heater> Heaters { get; set; }
        public DbSet<SensorReading> SensorReadings { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<MonthlyCostReport> MonthlyCostReports { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens {  get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<House>(e =>
            {
                e.HasOne(h => h.Owner).WithMany(o => o.Houses).HasForeignKey(h => h.OwnerId).OnDelete(DeleteBehavior.Cascade);

                e.HasMany(h => h.Heaters).WithOne(ht => ht.House).HasForeignKey(ht => ht.HouseId).OnDelete(DeleteBehavior.Cascade);

                e.HasMany(h => h.MonthlyCostReports).WithOne(r => r.House).HasForeignKey(r => r.HouseId).OnDelete(DeleteBehavior.Cascade);

            });


            modelBuilder.Entity<Heater>().HasMany(h => h.SensorReadings).WithOne(r => r.Heater).
                HasForeignKey(r => r.HeaterId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>().HasMany(u => u.RefreshTokens).WithOne(rt => rt.User)
                .HasForeignKey(rf => rf.UserId) .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
