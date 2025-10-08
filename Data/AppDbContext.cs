using CaffePOS.Model;
using CaffePOS.Model.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CaffePOS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets
        public DbSet<Category> Category { get; set; }
        public DbSet<Items> Items { get; set; }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is ITimestamped &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (ITimestamped)entry.Entity;
                var now = DateTime.Now;

                if (entry.State == EntityState.Added)
                {
                    entity.created_at = now;
                    entity.updated_at = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entity.updated_at = now;
                }
            }
        }

        //THÊM PHẦN NÀY VÀO CUỐI CLASS
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Items>()
                .ToTable(tb => tb.HasTrigger("TR_Items_Update")); 

            base.OnModelCreating(modelBuilder);
        }
    }
}
