using System.Collections.Generic;
using System.Threading.Tasks;
using Recetas.Core.Entities;

namespace Recetas.Core.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<Usuario?> GetByIdAsync(int id);
        Task InsertAsync(Usuario usuario);
        Task UpdateAsync(Usuario usuario);
        Task DeleteAsync(Usuario usuario);

        // Métodos adicionales opcionales
        Task<Usuario?> GetByCorreoAsync(string correo);
        Task<IEnumerable<Usuario>> GetActiveUsersAsync();
    }
}
