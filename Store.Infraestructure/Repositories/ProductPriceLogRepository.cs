using Store.Core.Entities;
using Store.Core.Interfaces.Repositories;
using Store.Infraestructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Infraestructure.Repositories
{
    public class ProductPriceLogRepository : BaseRepository<ProductPriceLog>, IProductPriceLogRepository
    {
        public ProductPriceLogRepository(StoreDbContext context) : base(context)
        {
        }
    }
}
