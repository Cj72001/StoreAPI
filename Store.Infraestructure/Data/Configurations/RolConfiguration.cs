using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Core.Entities;

namespace Store.Infraestructure.Data.Configurations
{
    public class RolConfiguration : IEntityTypeConfiguration<Rol>
    {
        public void Configure(EntityTypeBuilder<Rol> entity)
        {
            entity.ToTable("Rol");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Name)
                  .HasColumnName("name")
                  .HasMaxLength(50)
                  .IsRequired();

            // Datos iniciales
            entity.HasData(
                new Rol { Id = 1, Name = "User" },
                new Rol { Id = 2, Name = "Admin" }
            );

        }
    }
}
