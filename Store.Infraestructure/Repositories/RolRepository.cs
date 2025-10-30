using Store.Core.Entities;
using Store.Core.Interfaces.Repositories;
using Store.Infraestructure.Data;

namespace Store.Infraestructure.Repositories
{
    public class RolRepository : BaseRepository<Rol>, IRolRepository
    {
        public RolRepository(StoreDbContext context) : base(context)
        {
        }
    }
}
