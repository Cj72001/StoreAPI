using Microsoft.Data.SqlClient;
using Store.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Interfaces.Repositories
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task<List<T>> GetAllAsync(string spName, params SqlParameter[] parameters);
        Task<T?> GetSingleAsync(string spName, params SqlParameter[] parameters);
        Task<int> ExecuteAsync(string spName, params SqlParameter[] parameters);

    }
}
