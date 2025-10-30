using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Core.Entities;

namespace Store.Infraestructure.Data.Configurations
{
    public class ProductPriceLogConfiguration : IEntityTypeConfiguration<ProductPriceLog>
    {
        public void Configure(EntityTypeBuilder<ProductPriceLog> entity)
        {
            entity.ToTable("Product_price_log");

            entity.HasKey(e => e.Id);

            // Propiedades

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.OldPrice)
                  .HasColumnName("old_price")
                  .HasColumnType("decimal(20,8)")
                  .IsRequired();

            entity.Property(e => e.NewPrice)
                  .HasColumnName("new_price")
                  .HasColumnType("decimal(20,8)")
                  .IsRequired();

            entity.Property(e => e.ChangedOnUtc)
                  .HasColumnName("changed_on_utc")
                  .HasDefaultValueSql("SYSUTCDATETIME()")
                  .IsRequired();

            entity.Property(e => e.ChangedBy)
                  .HasColumnName("changed_by")
                  .IsRequired();

            entity.Property(e => e.ProductId)
                  .HasColumnName("product_id")
                  .IsRequired();

            // Relaciones

            entity.HasOne<User>()
                  .WithMany(u => u.ProductPriceLogs)
                  .HasForeignKey(p => p.ChangedBy);

            entity.HasOne<Product>()
                  .WithMany(p => p.ProductPriceLogs)
                  .HasForeignKey(p => p.ProductId);
        }
    }
}
