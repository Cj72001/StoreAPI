using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Entities
{
    public class Purchase : BaseEntity
    {
        public decimal Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime PurchasedOnUtc { get; set; }
        public int ProductId { get; set; } // FK a Product
        public int PurchasedBy { get; set; } // FK a User

        // Relaciones
        public Product Product { get; set; }
        public User PurchasedByUser { get; set; }
    }
}
