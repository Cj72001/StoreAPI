using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IProductRepository Products { get; }
        IProductPriceLogRepository ProductPriceLogs { get; }
        IPurchaseRepository Purchases { get; }

        Task<int> SaveChangesAsync(); // Para confirmar transacciones
    }
}
