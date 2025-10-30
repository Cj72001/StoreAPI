using Store.Core.Interfaces;
using Store.Core.Interfaces.Repositories;
using Store.Infraestructure.Data;
using Store.Infraestructure.Repositories;

namespace Store.Infraestructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreDbContext _context;

        public UnitOfWork(StoreDbContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Products = new ProductRepository(_context);
            ProductPriceLogs = new ProductPriceLogRepository(_context);
            Purchases = new PurchaseRepository(_context);
        }

        public IUserRepository Users { get; private set; }
        public IProductRepository Products { get; private set; }
        public IProductPriceLogRepository ProductPriceLogs { get; private set; }
        public IPurchaseRepository Purchases { get; private set; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
