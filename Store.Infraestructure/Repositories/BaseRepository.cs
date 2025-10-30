using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Store.Core.Entities;
using Store.Infraestructure.Data;
using Store.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Store.Core.Interfaces.Repositories;

namespace Store.Infraestructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly StoreDbContext _context;

        public BaseRepository(StoreDbContext context)
        {
            _context = context;
        }

        // Ejecuta un SP que devuelve un listado de entidades T
        public async Task<List<T>> GetAllAsync(string spName, params SqlParameter[] parameters)
        {
            return await _context.Set<T>()
                .FromSqlRaw(BuildSql(spName, parameters), parameters)
                .ToListAsync();
        }

        // Ejecuta un SP que devuelve una unica entidad T
        public async Task<T?> GetSingleAsync(string spName, params SqlParameter[] parameters)
        {
            return _context.Set<T>()
                .FromSqlRaw(BuildSql(spName, parameters), parameters)
                .AsEnumerable() // evita composicion SQL
                .FirstOrDefault();
        }

        // Ejecuta un SP que realiza INSERT, UPDATE o DELETE
        public async Task<int> ExecuteAsync(string spName, params SqlParameter[] parameters)
        {
            return await _context.Database.ExecuteSqlRawAsync(BuildSql(spName, parameters), parameters);
        }

        // Construye la cadena EXEC SP con parametros
        private string BuildSql(string spName, SqlParameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
                return $"EXEC {spName}";

            var paramNames = parameters.Select(p => p.ParameterName).ToArray();
            return $"EXEC {spName} {string.Join(", ", paramNames)}";
        }
    }
}
