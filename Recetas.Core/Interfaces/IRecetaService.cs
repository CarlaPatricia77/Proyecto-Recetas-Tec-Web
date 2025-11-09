using Recetas.Core.Entities;
using Recetas.Core.QueryFilters;

namespace Recetas.Core.Interfaces
{
    /// <summary>
    /// Servicio de lógica de negocio para Recetas
    /// </summary>
    public interface IRecetaService
    {
        IEnumerable<Receta> GetAllRecetas(RecetaQueryFilter filters);
        Task<Receta> GetRecetaById(int id);
        Task InsertReceta(Receta receta);
        Task UpdateReceta(Receta receta);
        Task DeleteReceta(int id);
    }
}
