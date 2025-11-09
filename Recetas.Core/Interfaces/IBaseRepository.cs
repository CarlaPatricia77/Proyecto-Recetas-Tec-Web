using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Recetas.Core.Entities;

namespace Recetas.Core.Interfaces
{
    /// <summary>
    /// Interfaz genérica para operaciones CRUD básicas
    /// </summary>
    /// <typeparam name="T">Tipo de entidad que hereda de BaseEntity</typeparam>
    public interface IBaseRepository<T> where T : BaseEntity
    {
        IEnumerable<T> GetAll();
        Task<T?> GetById(int id);
        Task Add(T entity);
        void Update(T entity);
        Task Delete(int id);
    }
}
