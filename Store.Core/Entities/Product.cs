using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Stock { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOnUtc { get; set; }
        public DateTime? UpdatedStockOnUtc { get; set; }

        // Relaciones
        public ICollection<ProductPriceLog> ProductPriceLogs { get; set; } = new List<ProductPriceLog>();
        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    }
}
