using System.Collections.Generic;
using System.Threading.Tasks;
using Recetas.Core.Entities;

namespace Recetas.Core.Interfaces
{
    public interface ICategoriasRepository
    {
        Task<IEnumerable<Categoria>> GetAllAsync();
        Task<Categoria?> GetByIdAsync(int id);
        Task InsertAsync(Categoria categoria);
        Task UpdateAsync(Categoria categoria);
        Task DeleteAsync(Categoria categoria);
        Task<Categoria?> GetByNombreAsync(string nombre);

        // Nuevos métodos específicos del negocio
        Task<IEnumerable<Categoria>> GetCategoriasConMasDeNRecetasAsync(int cantidad);
        Task<IEnumerable<Categoria>> BuscarCategoriasPorDescripcionAsync(string texto);
        //metodos agregadoos
        Task<IEnumerable<Categoria>> GetCategoriasPorFecha(
            DateTime fechaInicio,
            DateTime fechaFin);

    }
}
