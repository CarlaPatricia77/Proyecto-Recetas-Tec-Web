using Microsoft.EntityFrameworkCore;
using Recetas.Core.Entities;
using Recetas.Core.Interfaces;
using Recetas.Infrastructure.Data;

namespace Recetas.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación genérica del repositorio base
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        private readonly RecetasContext _context;
        protected readonly DbSet<T> _entities;

        public BaseRepository(RecetasContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }

        public IEnumerable<T> GetAll()
        {
            return _entities.AsEnumerable();
        }

        public async Task<T?> GetById(int id)
        {
            return await _entities.FindAsync(id);
        }

        public async Task Add(T entity)
        {
            await _entities.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _entities.Update(entity);
        }

        public async Task Delete(int id)
        {
            T? entity = await GetById(id);
            if (entity != null)
            {
                _entities.Remove(entity);
            }
        }
    }
}