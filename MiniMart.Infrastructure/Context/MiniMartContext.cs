using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MiniMart.Domain.Entities;

namespace MiniMart.Infrastructure.Context
{
    public class MiniMartContext : DbContext
    {
        public MiniMartContext(DbContextOptions<MiniMartContext> option) : base(option) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<CallBackEventLog> CallBackEventLogs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasIndex(e => e.TransactionReference);
            });

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType) && !entityType.ClrType.IsAbstract)
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property(nameof(BaseEntity.Id))
                        .HasDefaultValueSql("NEWSEQUENTIALID()");
                }

                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                    {
                        property.SetColumnType("decimal(18,2)");
                    }
                }
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var item in ChangeTracker.Entries<BaseEntity>())
            {
                switch (item.State)
                {
                    case EntityState.Modified:
                        item.Entity.LastModifiedDate = DateTime.UtcNow;
                        break;
                    case EntityState.Added:
                        item.Entity.CreationDate = DateTime.UtcNow;
                        item.Entity.CreatedBy ??= "SYSTEM";
                        break;
                    default:
                        break;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
