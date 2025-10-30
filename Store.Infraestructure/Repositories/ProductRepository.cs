using Store.Core.Entities;
using Store.Core.Interfaces.Repositories;
using Store.Infraestructure.Data;

namespace Store.Infraestructure.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(StoreDbContext context) : base(context)
        {
        }
    }
}
