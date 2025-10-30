using Microsoft.EntityFrameworkCore;
using Store.Core.DTOs;
using Store.Core.Entities;
using Store.Infraestructure.Data.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Store.Infraestructure.Data
{
    public class StoreDbContext : DbContext
    {
        public StoreDbContext(DbContextOptions<StoreDbContext> options)
            : base(options)
        {
        }

        // DbSets (Tablas)
        public DbSet<Rol> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductPriceLog> ProductPriceLogs { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<UserDTO> UserDTOs { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Aplica todas las configuraciones automaticamente desde este assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(StoreDbContext).Assembly);

            modelBuilder.Entity<UserDTO>(eb =>
            {
                eb.HasNoKey();      // Indica que no es una entidad completa
                eb.ToView(null);    // No esta ligada a ninguna tabla
            });

        }
    }
}
