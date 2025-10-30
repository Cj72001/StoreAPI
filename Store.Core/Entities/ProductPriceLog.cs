using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Entities
{
    public class ProductPriceLog : BaseEntity
    {
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public DateTime ChangedOnUtc { get; set; }
        public int ChangedBy { get; set; } // FK a User
        public int ProductId { get; set; } // FK a Product


        // Relaciones
        public User ChangedByUser { get; set; }
        public Product Product { get; set; }
    }
}
