using Microsoft.EntityFrameworkCore;
using MiniSO.Domain.Entities;

namespace MiniSO.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<SalesOrderDetail> SalesOrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Client>(entity =>
            {
                entity.ToTable("Client");
                entity.HasKey(e => e.ClientId);
                entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(200);
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.ToTable("Item");
                entity.HasKey(e => e.ItemId);
                entity.Property(e => e.ItemCode).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.ItemCode).IsUnique();
            });

            modelBuilder.Entity<SalesOrder>(entity =>
            {
                entity.ToTable("SalesOrder");
                entity.HasKey(e => e.SalesOrderId);
                entity.Property(e => e.InvoiceNo).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.InvoiceNo).IsUnique();
                entity.HasOne(e => e.Client)
                    .WithMany(c => c.SalesOrders)
                    .HasForeignKey(e => e.ClientId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SalesOrderDetail>(entity =>
            {
                entity.ToTable("SalesOrderDetail");
                entity.HasKey(e => e.SalesOrderDetailId);
                entity.HasOne(e => e.SalesOrder)
                    .WithMany(so => so.SalesOrderDetails)
                    .HasForeignKey(e => e.SalesOrderId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Item)
                    .WithMany(i => i.SalesOrderDetails)
                    .HasForeignKey(e => e.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}