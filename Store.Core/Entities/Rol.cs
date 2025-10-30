using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Entities
{
    public class Rol : BaseEntity
    {
        public string Name { get; set; }

        // Relaciones
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
