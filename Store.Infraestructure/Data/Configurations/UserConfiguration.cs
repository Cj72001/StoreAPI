using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Core.Entities;

namespace Store.Infraestructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            entity.ToTable("User");

            entity.HasKey(e => e.Id);

            // Propiedades

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Name)
                  .HasColumnName("name")
                  .HasMaxLength(100)
                  .IsRequired();

            entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(100)
                    .IsRequired();

            entity.Property(e => e.Password)
                  .HasColumnName("password")
                  .HasMaxLength(200)
                  .IsRequired();

            entity.Property(e => e.CreatedOnUtc)
                  .HasColumnName("created_on_utc")
                  .HasDefaultValueSql("SYSUTCDATETIME()")
                  .IsRequired();

            entity.Property(e => e.Rol)
                  .HasColumnName("rol")
                  .HasDefaultValue(1)
                  .IsRequired();

            // Relaciones

            entity.HasOne<Rol>()
                  .WithMany(r => r.Users)
                  .HasForeignKey(u => u.Rol);
        }
    }
}
