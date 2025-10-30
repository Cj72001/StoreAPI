using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Core.Entities;

namespace Store.Infraestructure.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> entity)
        {
            entity.ToTable("Product");

            entity.HasKey(e => e.Id);

            // Propiedades

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Name)
                  .HasColumnName("name")
                  .HasMaxLength(100)
                  .IsRequired();

            entity.Property(e => e.Description)
                  .HasColumnName("description")
                  .HasColumnType("nvarchar(max)");

            entity.Property(e => e.Stock)
                  .HasColumnName("stock")
                  .HasColumnType("decimal(20,8)")
                  .IsRequired();

            entity.Property(e => e.UpdatedStockOnUtc)
                  .HasColumnName("updated_stock_on_utc")
                  .IsRequired(false);

            entity.Property(e => e.UnitPrice)
                  .HasColumnName("unit_price")
                  .HasColumnType("decimal(20,8)")
                  .IsRequired();

            entity.Property(e => e.CreatedOnUtc)
                  .HasColumnName("created_on_utc")
                  .HasDefaultValueSql("SYSUTCDATETIME()")
                  .IsRequired();

            entity.Property(e => e.IsDeleted)
                  .HasColumnName("is_deleted")
                  .HasDefaultValue(false)
                  .IsRequired();

            entity.Property(e => e.DeletedOnUtc)
                  .HasColumnName("deleted_on_utc")
                  .IsRequired(false);

        }
    }
}
