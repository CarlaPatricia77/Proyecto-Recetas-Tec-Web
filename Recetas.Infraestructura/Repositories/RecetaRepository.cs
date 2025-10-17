using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Recetas.Core.Entities;
using Recetas.Core.Interfaces;
using Recetas.Infrastructure.Data;

namespace Recetas.Infrastructure.Repositories
{
    public class RecetaRepository : IRecetasRepository
    {
        private readonly RecetasContext _ctx;
        public RecetaRepository(RecetasContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<Receta>> GetAllAsync() =>
            await _ctx.Recetas
                .Include(r => r.Categoria)
                .Include(r => r.Usuario)
                .ToListAsync();

        public async Task<Receta?> GetByIdAsync(int id) =>
            await _ctx.Recetas
                .Include(r => r.Categoria)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(r => r.Id == id);

        public async Task InsertAsync(Receta receta)
        {
            _ctx.Recetas.Add(receta);
            await _ctx.SaveChangesAsync();
        }

        public async Task UpdateAsync(Receta receta)
        {
            _ctx.Recetas.Update(receta);
            await _ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(Receta receta)
        {
            _ctx.Recetas.Remove(receta);
            await _ctx.SaveChangesAsync();
        }

        // ============================
        //  CASOS DE USO DE NEGOCIO
        // ============================

        // Buscar recetas por categoría
        public async Task<IEnumerable<Receta>> GetByCategoriaAsync(int categoriaId) =>
            await _ctx.Recetas
                .Include(r => r.Categoria)
                .Include(r => r.Usuario)
                .Where(r => r.CategoriaId == categoriaId)
                .ToListAsync();

        // Buscar recetas por usuario
        public async Task<IEnumerable<Receta>> GetByUsuarioAsync(int usuarioId) =>
            await _ctx.Recetas
                .Include(r => r.Categoria)
                .Include(r => r.Usuario)
                .Where(r => r.UsuarioId == usuarioId)
                .ToListAsync();

        // Buscar recetas que contengan cierto ingrediente (campo texto)
        public async Task<IEnumerable<Receta>> BuscarPorIngredienteAsync(string ingrediente) =>
            await _ctx.Recetas
                .Where(r => r.Ingredientes.Contains(ingrediente))
                .ToListAsync();
    }
}
