using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Core.Entities;

namespace Store.Infraestructure.Data.Configurations
{
    public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
    {
        public void Configure(EntityTypeBuilder<Purchase> entity)
        {
            entity.ToTable("Purchase");

            entity.HasKey(e => e.Id);

            // Propiedades

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Quantity)
                  .HasColumnName("quantity")
                  .HasColumnType("decimal(20,8)")
                  .IsRequired();

            entity.Property(e => e.TotalAmount)
                  .HasColumnName("total_amount")
                  .HasColumnType("decimal(20,8)")
                  .IsRequired();

            entity.Property(e => e.PurchasedOnUtc)
                  .HasColumnName("purchased_on_utc")
                  .HasDefaultValueSql("SYSUTCDATETIME()")
                  .IsRequired();

            entity.Property(e => e.ProductId)
                  .HasColumnName("product_id")
                  .IsRequired();

            entity.Property(e => e.PurchasedBy)
                  .HasColumnName("purchased_by")
                  .IsRequired();

            // Relaciones
            entity.HasOne<Product>()
                  .WithMany(p => p.Purchases)
                  .HasForeignKey(p => p.ProductId)
                  .HasConstraintName("FK_Purchase_Product");

            entity.HasOne<User>()
                  .WithMany(p => p.Purchases)
                  .HasForeignKey(p => p.PurchasedBy)
                  .HasConstraintName("FK_Purchase_User");
        }
    }
}
