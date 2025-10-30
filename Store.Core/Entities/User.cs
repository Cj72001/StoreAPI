using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public int? Rol { get; set; } // FK a Rol

        // Relaciones
        public ICollection<ProductPriceLog> ProductPriceLogs { get; set; } = new List<ProductPriceLog>();
        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    }
}
