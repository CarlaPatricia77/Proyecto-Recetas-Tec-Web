using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Recetas.Core.Entities;

namespace Recetas.Core.Interfaces
{
    /// <summary>
    /// Patrón Unit of Work para coordinar transacciones
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IRecetasRepository RecetaRepository { get; }
        IBaseRepository<Usuario> UsuarioRepository { get; }
        IBaseRepository<Categoria> CategoriaRepository { get; }

        void SaveChanges();
        Task SaveChangesAsync();
    }
}