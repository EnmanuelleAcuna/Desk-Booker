using DeskBooker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using System;

namespace DeskBooker.DataAccess {
    public class DeskBookerContext : DbContext {
        public DeskBookerContext(DbContextOptions<DeskBookerContext> Options) : base(Options) { }

        public DbSet<DeskBooking> DeskBooking { get; set; }
        public DbSet<Desk> Desk { get; set; }

        protected override void OnModelCreating(ModelBuilder ModelBuilder) {
            SeedData(ModelBuilder);
        }

        private void SeedData(ModelBuilder ModelBuilder) {
            ModelBuilder.Entity<Desk>().HasData(
                 new Desk { Id = 1, Description = "Desk 1" },
                 new Desk { Id = 2, Description = "Desk 2" }
            );
        }
    }
}
