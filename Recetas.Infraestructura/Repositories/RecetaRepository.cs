using Microsoft.EntityFrameworkCore;
using Recetas.Core.Entities;
using Recetas.Core.Interfaces;
using Recetas.Infrastructure.Data;
using Recetas.Core.DTOs;

namespace Recetas.Infrastructure.Repositories
{
    /// <summary>
    /// Repositorio específico para Recetas con métodos de negocio adicionales
    /// </summary>
    public class RecetaRepository : BaseRepository<Receta>, IRecetasRepository
    {
        public RecetaRepository(RecetasContext context) : base(context)
        {
        }

        // Sobrescribir GetAll para incluir relaciones
        public new async Task<IEnumerable<Receta>> GetAllAsync() =>
            await _entities
                .Include(r => r.Categoria)
                .Include(r => r.Usuario)
                .ToListAsync();

        public async Task<Receta?> GetByIdAsync(int id) =>
            await _entities
                .Include(r => r.Categoria)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(r => r.Id == id);

        public async Task InsertAsync(Receta receta)
        {
            await Add(receta);
        }

        public async Task UpdateAsync(Receta receta)
        {
            Update(receta);
        }

        public async Task DeleteAsync(Receta receta)
        {
            await Delete(receta.Id);
        }

        // ============================
        //  CASOS DE USO DE NEGOCIO
        // ============================

        public async Task<IEnumerable<Receta>> GetByCategoriaAsync(int categoriaId) =>
            await _entities
                .Include(r => r.Categoria)
                .Include(r => r.Usuario)
                .Where(r => r.categoria_id == categoriaId)
                .ToListAsync();

        public async Task<IEnumerable<Receta>> GetByUsuarioAsync(int usuarioId) =>
            await _entities
                .Include(r => r.Categoria)
                .Include(r => r.Usuario)
                .Where(r => r.usuarioId == usuarioId)
                .ToListAsync();

        public async Task<IEnumerable<Receta>> BuscarPorIngredienteAsync(string ingrediente) =>
            await _entities
                .Where(r => r.ingredientes.Contains(ingrediente))
                .ToListAsync();
        public async Task<IEnumerable<RecetaResumenDto>> GetRecetasPorRangoFechasAsync(DateTime desde, DateTime hasta)
        {
            return await _entities
                .Include(r => r.Categoria)
                .Include(r => r.Usuario)
                .Where(r => r.fecha_creacion >= desde && r.fecha_creacion <= hasta)
                .Select(r => new RecetaResumenDto
                {
                    NombreReceta = r.titulo,
                    Categoria = r.Categoria != null ? r.Categoria.nombre : null,
                    Usuario = r.Usuario != null ? r.Usuario.nombre : null
                })
                .ToListAsync();
        }



    }
}