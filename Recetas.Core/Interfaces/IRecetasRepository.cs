using Recetas.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Recetas.Core.Interfaces
{
    public interface IRecetasRepository
    {
        Task<IEnumerable<Receta>> GetAllAsync();
        Task<Receta?> GetByIdAsync(int id);
        Task InsertAsync(Receta receta);
        Task UpdateAsync(Receta receta);
        Task DeleteAsync(Receta receta);

        // ====== Casos de uso de negocio ======
        Task<IEnumerable<Receta>> GetByCategoriaAsync(int categoriaId);
        Task<IEnumerable<Receta>> GetByUsuarioAsync(int usuarioId);
        Task<IEnumerable<Receta>> BuscarPorIngredienteAsync(string ingrediente);
    }
}
