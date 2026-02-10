using Microsoft.EntityFrameworkCore;
using SteelRollsWarehouse.Domain.Entities;

namespace SteelRollsWarehouse.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Roll> Rolls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Roll>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Length).IsRequired();
                entity.Property(e => e.Weight).IsRequired();
                entity.Property(e => e.AddedDate).IsRequired();
                entity.Property(e => e.DeletedDate).IsNullable();
                
                entity.HasIndex(e => e.AddedDate);
                entity.HasIndex(e => e.DeletedDate);
            });
        }
    }
}