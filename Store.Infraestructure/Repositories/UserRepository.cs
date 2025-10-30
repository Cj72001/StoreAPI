using Store.Core.Entities;
using Store.Core.Interfaces.Repositories;
using Store.Infraestructure.Data;

namespace Store.Infraestructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(StoreDbContext context) : base(context)
        {
        }
    }
}
